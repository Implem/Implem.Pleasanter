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
                        hb.Guide(text: ss.GridGuide);
                        break;
                    case "new":
                        hb.Guide(text: ss.EditorGuide);
                        break;
                    case "edit":
                        hb.Guide(
                            text: ss.EditorGuide,
                            _using: !ss.IsSite(context: context));
                        break;
                }
            });
        }

        private static HtmlBuilder Guide(this HtmlBuilder hb, string text, bool _using = true)
        {
            return _using && !text.IsNullOrEmpty()
                ? hb.Div(action: () => hb
                    .Div(css: "markup", action: () => hb
                        .Text(text: text)))
                : hb;
        }
    }
}