using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Views;
using System;
using System.Data;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class WorkValue : IConvertable
    {
        public decimal Value;
        public decimal ProgressRate;

        public WorkValue()
        {
        }

        public WorkValue(
            DataRow dataRow,
            string progressRateColumnName = "ProgressRate",
            string valueColumnName = "WorkValue")
        {
            ProgressRate = dataRow.Decimal(progressRateColumnName);
            Value = dataRow.Decimal(valueColumnName);
        }

        public WorkValue(decimal value, decimal progressRate)
        {
            Value = value;
            ProgressRate = progressRate;
        }

        public string ToControl(Column column)
        {
            return Value.ToString();
        }

        public string ToResponse()
        {
            return Value.ToString();
        }

        public HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            var width = column.Max != null
                ? Convert.ToInt32(Value / column.Max.ToInt() * 100)
                : 0;
            return hb.Td(action: () => hb
                .Svg(css: "svg-work-value", action: () => hb
                    .SvgText(
                        text: column.Format(Value) + column.Unit,
                        x: 0,
                        y: Parameters.WorkValueTextTop)
                    .Rect(
                        x: 0,
                        y: Parameters.WorkValueHeight / 2,
                        width: width,
                        height: Parameters.WorkValueHeight / 2)
                    .Rect(
                        x: 0,
                        y: Parameters.WorkValueHeight / 2,
                        width: ProgressRate != 0
                            ? (int)(width * (ProgressRate / 100))
                            : 0,
                        height: Parameters.WorkValueHeight / 2)));
        }

        public string ToExport(Column column)
        {
            return Value.ToString();
        }
    }
}