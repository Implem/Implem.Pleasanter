using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Server;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlStyles
    {
        public static HtmlBuilder Styles(this HtmlBuilder hb, string style)
        {
            return Contract.Style()
                ? hb.Style(type: "text/css", style: style)
                : hb;
        }
    }
}