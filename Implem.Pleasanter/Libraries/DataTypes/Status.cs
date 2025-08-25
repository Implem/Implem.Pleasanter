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
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
using System.Linq;
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

        public string ToControl(Context context, SiteSettings ss, Column column)
        {
            var value = Value.ToString();
            return column.ChoiceHash?.ContainsKey(value) == true
                || Value != 0
                    ? value
                    : string.Empty;
        }

        public string ToResponse(Context context, SiteSettings ss, Column column)
        {
            return !column.GetEditorReadOnly()
                ? Value.ToString()
                : column.Choice(ToString()).Text;
        }

        public string ToDisplay(Context context, SiteSettings ss, Column column)
        {
            return column.Choice(ToString()).Text;
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
                    return Value.ToString();
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
                attributes: new HtmlAttributes()
                    .DataCellSticky(column.CellSticky)
                    .DataCellWidth(column.CellWidth),
                action: () =>
                {
                    StyleBody(
                        hb: hb,
                        column: column);
                });
        }

        public HtmlBuilder StyleBody(
            HtmlBuilder hb,
            Column column,
            string tag = "P")
        {
            var choice = column.Choice(Value.ToString());
            var attributes = new HtmlAttributes()
                .Class(choice.CssClass)
                .Style(choice.Style);
            var text = column.ChoiceHash.Get(Value.ToString()) == null
                ? Value == 0
                    ? null
                    : "?" + Value
                : choice.TextMini;
            switch (tag)
            {
                case "P":
                    return hb.P(
                        attributes: attributes,
                        action: () => hb
                            .Text(text: text));
                case "SPAN":
                    return hb.Span(
                        attributes: attributes,
                        action: () => hb
                            .Text(text: text));
                default:
                    return null;
            }
        }

        public object ToApiDisplayValue(Context context, SiteSettings ss, Column column)
        {
            return column.Choice(ToString()).Text;
        }

        public object ToApiValue(Context context, SiteSettings ss, Column column)
        {
            return Value;
        }

        public string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            return Value == 0
                ? null
                : column.ChoiceParts(
                    context: context,
                    selectedValues: ToString(),
                    type: exportColumn?.Type ?? ExportColumn.Types.Text)
                        .FirstOrDefault();
        }

        public bool InitialValue(Context context)
        {
            return Value == 0;
        }

        public string GridText(Context context, Column column)
        {
            return column.Choice(ToString()).TextMini;
        }

        public string ToNotice(
            Context context,
            int saved,
            Column column,
            NotificationColumnFormat notificationColumnFormat,
            bool updated,
            bool update)
        {
            return notificationColumnFormat.DisplayText(
                self: column.Choice(Value.ToString()).Text,
                saved: column.Choice(saved.ToString()).Text,
                column: column,
                updated: updated,
                update: update);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public bool Incomplete()
        {
            return Value < Parameters.General.CompletionCode;
        }
    }
}