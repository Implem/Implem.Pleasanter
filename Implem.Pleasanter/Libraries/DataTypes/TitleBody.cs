using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class TitleBody : Title, IConvertable
    {
        public string Body = string.Empty;

        public TitleBody()
        {
        }

        public TitleBody(long id, int ver, bool isHistory, string title, string displayValue, string body)
        {
            Id = id;
            Ver = ver;
            Value = title;
            IsHistory = isHistory;
            DisplayValue = displayValue;
            Body = body;
        }

        public override string ToDisplay(Context context, SiteSettings ss, Column column)
        {
            return $"{DisplayValue}\n{Body}";
        }

        public override string ToString()
        {
            return Value + "\r\n" + Body;
        }

        public override HtmlBuilder Td(
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
                    .DataCellWidth(column.CellWidth)
                    .DataCellWordWrap(column.CellWordWrap),
                action: () => TdTitleBody(
                    hb: hb,
                    context: context,
                    column: column,
                    tabIndex: tabIndex));
        }

        private HtmlBuilder TdTitleBody(
            HtmlBuilder hb,
            Context context,
            Column column,
            int? tabIndex = null)
        {
            return hb.Div(css: "grid-title-body", action: () => hb
                .P(css: "title", action: () => TdTitle(
                    hb: hb,
                    context: context,
                    column: column,
                    tabIndex: tabIndex))
                .Div(
                    css: "body",
                    action: () => {
                        switch (column.ControlType)
                        {
                            case "RTEditor":
                                hb.RichTextEditor(action: () => hb.TextArea(
                                    disabled: true,
                                    attributes: new HtmlAttributes().Add("data-enablelightbox", Implem.DefinitionAccessor.Parameters.General.EnableLightBox ? "1" : "0"),
                                    text: Body));
                                break;
                            default:
                                hb.Div(
                                    css: "markup",
                                    attributes: new HtmlAttributes().Add("data-enablelightbox", Implem.DefinitionAccessor.Parameters.General.EnableLightBox ? "1" : "0"),
                                    action: () => hb.Text(text: Body)
                                );
                                break;
                        }
                    }));
        }

        public override string GridText(Context context, Column column)
        {
            var hb = new HtmlBuilder();
            TdTitleBody(hb: hb, context: context, column: column);
            return hb.ToString();
        }

        public override object ToApiDisplayValue(Context context, SiteSettings ss, Column column)
        {
            return $"{DisplayValue}\n{Body}";
        }

        public override object ToApiValue(Context context, SiteSettings ss, Column column)
        {
            return $"{Value}\n{Body}";
        }

        public override string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            return ToString();
        }
    }
}