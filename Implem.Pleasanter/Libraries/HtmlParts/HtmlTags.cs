using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Resources;
using System;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlTags
    {
        public static HtmlBuilder Header(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "header",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Footer(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "footer",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Article(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "article",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Nav(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "nav",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Table(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "table",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Caption(
            this HtmlBuilder hb,
            string caption,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "caption",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: () => hb
                        .Text(text: caption))
                : hb;
        }

        public static HtmlBuilder THead(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "thead",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder TBody(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "tbody",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Th(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "th",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Tr(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "tr",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "td",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder H(
            this HtmlBuilder hb,
            int number,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "h" + number,
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Section(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "section",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Div(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "div",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder P(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "p",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Label(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "label",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Input(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "input",
                    closeLevel: 1,
                    attributes: (attributes ?? new HtmlAttributes())
                        .Id(id)
                        .Class(css))
                : hb;
        }

        public static HtmlBuilder TextArea(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            string placeholder = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "textarea",
                    id: id,
                    css: css,
                    attributes: (attributes ?? new HtmlAttributes())
                        .Id(id)
                        .Class(css)
                        .Placeholder(placeholder),
                    action: action)
                : hb;
        }

        public static HtmlBuilder Button(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "button",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Select(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "select",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Option(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "option",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Span(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "span",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Em(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "em",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Ul(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "ul",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Ol(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "ol",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Li(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "li",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder A(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            string href = null,
            string target = null,
            HtmlAttributes attributes = null,
            string text = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "a",
                    id: id,
                    css: css,
                    attributes: (attributes ?? new HtmlAttributes())
                        .Id(id)
                        .Class(css)
                        .Href(href)
                        .Target(target),
                    action: action != null
                        ? action
                        : () => hb.Text(text))
                : hb;
        }

        public static HtmlBuilder Form(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "form",
                    id: null,
                    css: null,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Time(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "time",
                    id: id,
                    css: css,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder FieldSet(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            string legendText = null,
            HtmlAttributes attributes = null,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "fieldset",
                    id: id,
                    css: Css.Class("fieldset cf", css),
                    attributes: attributes,
                    action: () =>
                    {
                        if (legendText != string.Empty)
                        {
                            hb.Append(
                                tag: "legend",
                                id: null,
                                css: "legend",
                                attributes: null,
                                action: () => hb.Text(legendText));
                        }
                        action?.Invoke();
                    })
                : hb;
        }

        public static HtmlBuilder Img(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            string src = null,
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "img",
                    attributes: new HtmlAttributes()
                        .Id(id)
                        .Class(css)
                        .Src(src),
                    closeLevel: 1)
                : hb;
        }

        public static HtmlBuilder Video(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "video",
                    attributes: new HtmlAttributes()
                        .Id(id)
                        .Class(css),
                    closeLevel: 1)
                : hb;
        }

        public static HtmlBuilder Canvas(
            this HtmlBuilder hb,
            string id = null,
            string css = null,
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "canvas",
                    attributes: new HtmlAttributes()
                        .Id(id)
                        .Class(css),
                    closeLevel: 1)
                : hb;
        }

        public static HtmlBuilder Link(
            this HtmlBuilder hb,
            string href = null,
            string rel = null,
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "link",
                    id: null,
                    css: null,
                    attributes: new HtmlAttributes()
                        .Href(href)
                        .Rel(rel),
                    action: null)
                : hb;
        }

        public static HtmlBuilder Style(
            this HtmlBuilder hb,
            string id = null,
            string src = null,
            string type = null,
            string style = null,
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "style",
                    id: null,
                    css: null,
                    attributes: new HtmlAttributes()
                        .Src(src)
                        .Type(type),
                    action: () => hb
                        .Raw(text: style))
                : hb;
        }

        public static HtmlBuilder Script(
            this HtmlBuilder hb,
            string id = null,
            string src = null,
            string script = null,
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "script",
                    id: id,
                    css: null,
                    attributes: new HtmlAttributes().Src(src),
                    action: () => hb
                        .Raw(text: script))
                : hb;
        }
    }
}