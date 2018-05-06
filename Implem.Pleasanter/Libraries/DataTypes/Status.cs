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
    public class Status : IConvertable
    {
        public int Value;

        public Status()
        {
        }

        public Status(DataRow dataRow, ColumnNameInfo column)
        {
            Value = dataRow.Int(Rds.DataColumnName(column, "Status"));
        }

        public Status(int value)
        {
            Value = value;
        }

        public string ToControl(SiteSettings ss, Column column)
        {
            return Value.ToString();
        }

        public string ToResponse()
        {
            return Value.ToString();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            var choice = column.Choice(Value.ToString());
            return hb.Td(action: () => hb
                .P(
                    attributes: new HtmlAttributes()
                        .Class(choice.CssClass)
                        .Style(choice.Style),
                    action: () => hb
                        .Text(column.ChoiceHash.Get(Value.ToString()) == null
                            ? Value == 0
                                ? null
                                : "?" + Value
                            : choice.TextMini)));
        }

        public bool Incomplete()
        {
            return Value < Parameters.General.CompletionCode;
        }

        public string GridText(Column column)
        {
            return column.Choice(ToString()).TextMini;
        }

        public string ToExport(Column column, ExportColumn exportColumn = null)
        {
            return Value == 0 && !column.ChoiceHash.ContainsKey(ToString())
                ? null
                : column.ChoicePart(ToString(), exportColumn?.Type ?? ExportColumn.Types.Text);
        }

        public string ToNotice(
            int saved,
            Column column,
            bool updated,
            bool update)
        {
            return column.Choice(Value.ToString()).Text.ToNoticeLine(
                column.Choice(saved.ToString()).Text,
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