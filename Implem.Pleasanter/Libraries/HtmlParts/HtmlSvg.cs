using Implem.Pleasanter.Libraries.Html;
using System;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlSvg
    {
        public static HtmlBuilder Svg(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            Action action = null,
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "svg",
                    id: id,
                    css: css,
                    attributes: new HtmlAttributes()
                        .Id(id)
                        .Class(css)
                        .Add("xmlns", "http://www.w3.org/2000/svg"),
                    action: action)
                : hb;
        }

        public static HtmlBuilder Rect(
            this HtmlBuilder hb,
            int? x = null,
            int? y = null,
            string width = null,
            string height = null,
            string fill = null,
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "rect",
                    id: null,
                    css: null,
                    attributes: new HtmlAttributes()
                        .Add("x", x.ToString(), _using: x != null)
                        .Add("y", y.ToString(), _using: y != null)
                        .Add("width", width, _using: width != null)
                        .Add("height", height, _using: height != null)
                        .Add("fill", fill, _using: fill != null),
                    action: () => { })
                : hb;
        }

        public static HtmlBuilder SvgText(
            this HtmlBuilder hb,
            string text,
            int? x = null,
            int? y = null,
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "text",
                    id: null,
                    css: null,
                    attributes: new HtmlAttributes()
                        .Add("x", x.ToString(), _using: x != null)
                        .Add("y", y.ToString(), _using: y != null),
                    action: () => hb
                        .Text(text: text))
                : hb;
        }
    }
}