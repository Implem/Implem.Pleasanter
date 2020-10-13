using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
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

        public override string ToString()
        {
            return Value + "\r\n" + Body;
        }

        public override HtmlBuilder Td(
            HtmlBuilder hb,
            Context context,
            Column column,
            int? tabIndex)
        {
            return hb.Td(
                css: column.CellCss(),
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
                .P(css: "body markup", action: () => hb
                        .Text(text: Body)));
        }

        public override string GridText(Context context, Column column)
        {
            var hb = new HtmlBuilder();
            TdTitleBody(hb: hb, context: context, column: column);
            return hb.ToString();
        }

        public override string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            return ToString();
        }
    }
}