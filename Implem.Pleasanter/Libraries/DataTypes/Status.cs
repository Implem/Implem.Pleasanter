using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Data;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class Status : IConvertable
    {
        public int Value;

        public Status()
        {
        }

        public Status(DataRow dataRow, string name)
        {
            Value = dataRow[name].ToInt();
        }

        public Status(int value)
        {
            Value = value;
        }

        public string ToControl(Column column, Permissions.Types pt)
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
            return hb.Td(action: () => hb
                .HtmlStatus(
                    column: column,
                    selectedValue: Value != 0
                        ? Value.ToString()
                        : string.Empty));
        }

        public bool Incomplete()
        {
            return Value < Parameters.General.CompletionCode;
        }

        public string GridText(Column column)
        {
            return column.Choice(ToString()).TextMini;
        }

        public string ToExport(Column column)
        {
            return column.Choice(ToString()).Text;
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
    }
}