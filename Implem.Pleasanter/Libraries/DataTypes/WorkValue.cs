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

        public string ToControl(SiteSettings ss, Column column)
        {
            return column.Display(ss, Value);
        }

        public string ToResponse()
        {
            return Value.ToString();
        }

        public HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return hb.Td(action: () => Svg(hb, column));
        }

        private HtmlBuilder Svg(HtmlBuilder hb, Column column)
        {
            var width = column.Max != null
                ? Convert.ToInt32(Value / column.Max.ToInt() * 100)
                : 0;
            return hb.Svg(css: "svg-work-value", action: () => hb
                .SvgText(
                    text: column.Display(Value, unit: true),
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

        public string GridText(Column column)
        {
            return column.Display(Value, unit: true);
        }

        public string ToExport(Column column, ExportColumn exportColumn = null)
        {
            return Value.ToString();
        }

        public string ToNotice(
            decimal saved,
            Column column,
            bool updated,
            bool update)
        {
            return column.Display(Value, unit: true).ToNoticeLine(
                column.Display(saved, unit: true),
                column,
                updated,
                update);
        }

        public bool InitialValue()
        {
            return Value == 0;
        }
    }
}