using Implem.Pleasanter.Libraries.Html;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlStyles
    {
        public static HtmlBuilder Styles(this HtmlBuilder hb, string style)
        {
            return hb.Style(type: "text/css", style: style);
        }
    }
}