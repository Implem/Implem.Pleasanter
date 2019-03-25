using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlGuides
    {
        public static HtmlBuilder Guide(this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Div(id: "Guide", action: () =>
            {
                switch (context.Action)
                {
                    case "index":
                    case "indexjson":
                        if (!ss.GridGuide.IsNullOrEmpty())
                        {
                            hb.Div(action: () => hb
                               .Div(css: "markup", action: () => hb
                                   .Text(text: ss.GridGuide)));
                        }
                        break;
                    case "new":
                    case "edit":
                        if (!ss.EditorGuide.IsNullOrEmpty() && !ss.IsSite(context: context))
                        {
                            hb.Div(action: () => hb
                               .Div(css: "markup", action: () => hb
                                   .Text(text: ss.EditorGuide)));
                        }
                        break;
                }
            });
        }
    }
}