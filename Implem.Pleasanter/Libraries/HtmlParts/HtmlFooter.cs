using Implem.Pleasanter.Libraries.Html;
using System;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlFooter
    {
        public static HtmlBuilder Footer(this HtmlBuilder hb)
        {
            return hb.Footer(id: "Footer", action: () => hb
                .P(action: () => hb
                    .A(
                        attributes: new HtmlAttributes().Href("https://implem.co.jp"),
                        action: () => hb
                            .Raw(text: "Copyright &copy; Implem Inc. 2014 - "
                                + DateTime.Now.Year))));
        }
    }
}