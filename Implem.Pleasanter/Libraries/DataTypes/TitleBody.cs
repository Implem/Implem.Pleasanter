using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class TitleBody : Title, IConvertable
    {
        public string Body = string.Empty;

        public TitleBody()
        {
        }

        public TitleBody(long id, string title, string displayValue, string body)
        {
            Id = id;
            Value = title;
            DisplayValue = displayValue;
            Body = body;
        }

        public override string ToString()
        {
            return Value + "\r\n" + Body;
        }

        public override HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return hb.Td(action: () => TdTitleBody(hb, column));
        }

        private HtmlBuilder TdTitleBody(HtmlBuilder hb, Column column)
        {
            return hb.Div(css: "grid-title-body", action: () => hb
                .P(css: "title", action: () => TdTitle(hb, column))
                .P(css: "body markup", action: () => hb
                        .Text(text: Body)));
        }

        public override string GridText(Column column)
        {
            var hb = new HtmlBuilder();
            TdTitleBody(hb, column);
            return hb.ToString();
        }

        public override string ToExport(Column column, ExportColumn exportColumn = null)
        {
            return ToString();
        }
    }
}