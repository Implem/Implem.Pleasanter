using Implem.Pleasanter.Libraries.Html;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlSiteMenu
    {
        public static HtmlBuilder StackStyles(this HtmlBuilder hb)
        {
            return hb
                .Div(css: "stacking1")
                .Div(css: "stacking2");
        }
    }
}