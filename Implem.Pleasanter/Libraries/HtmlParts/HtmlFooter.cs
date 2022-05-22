using Implem.DefinitionAccessor;
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
                        attributes: new HtmlAttributes().Href(Parameters.CopyrightUrl()),
                        action: () => hb
                            .Raw(text: Parameters.Copyright()))));
        }
    }
}