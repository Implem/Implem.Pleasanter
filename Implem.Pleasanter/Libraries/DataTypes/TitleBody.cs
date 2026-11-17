using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class TitleBody : Title, IConvertable
    {
        public string Body = string.Empty;
        private BaseModel model;

        public TitleBody()
        {
        }

        public TitleBody(
            long id,
            int ver,
            bool isHistory,
            string title,
            string displayValue,
            string body,
            BaseModel model = null)
        {
            Id = id;
            Ver = ver;
            Value = title;
            IsHistory = isHistory;
            DisplayValue = displayValue;
            Body = body;
            this.model = model;
        }

        private string ReadableBody(Context context, SiteSettings ss, Column column)
        {
            ss = column?.SiteSettings ?? ss;
            if (model == null || ss == null)
            {
                return Body;
            }
            return ss.GetColumn(
                context: context,
                columnName: "Body")?.CanRead(
                    context: context,
                    ss: ss,
                    mine: model.Mine(context: context),
                    noCache: true) != false
                        ? Body
                        : string.Empty;
        }

        public override string ToDisplay(Context context, SiteSettings ss, Column column)
        {
            return $"{DisplayValue}\n{ReadableBody(context: context, ss: ss, column: column)}";
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
            var body = ReadableBody(
                context: context,
                ss: null,
                column: column);
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
                                    text: body));
                                break;
                            default:
                                hb.MarkdownField(action: () => hb.TextArea(
                                    disabled: true,
                                    attributes: new HtmlAttributes().Add("data-enablelightbox", Implem.DefinitionAccessor.Parameters.General.EnableLightBox ? "1" : "0"),
                                    text: body)
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
            return $"{DisplayValue}\n{ReadableBody(context: context, ss: ss, column: column)}";
        }

        public override object ToApiValue(Context context, SiteSettings ss, Column column)
        {
            return $"{Value}\n{ReadableBody(context: context, ss: ss, column: column)}";
        }

        public override string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            return Value + "\r\n" + ReadableBody(
                context: context,
                ss: null,
                column: column);
        }
    }
}