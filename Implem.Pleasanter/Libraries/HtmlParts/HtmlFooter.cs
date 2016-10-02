using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
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
                        attributes: new HtmlAttributes().Href(Parameters.General.HtmlCopyrightUrl),
                        action: () => hb
                            .Raw(Parameters.General.HtmlCopyright.Params(DateTime.Now.Year)))));
        }
    }
}