using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Styles;
using System;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlTags
    {
        public static HtmlBuilder Header(
            this HtmlBuilder hb,
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "header",
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Footer(
            this HtmlBuilder hb,
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "footer",
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Article(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "article",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Article(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "article",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Nav(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "nav",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Nav(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "nav",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Table(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "table",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Table(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "table",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Caption(
            this HtmlBuilder hb,
            string caption,
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "caption",
                    action: () => hb
                        .Text(text: caption))
                : hb;
        }

        public static HtmlBuilder Th(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "th",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Th(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "th",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Tr(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "tr",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }
       
        public static HtmlBuilder Tr(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "tr",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Td(
                 this HtmlBuilder hb,
                 string id = "",
                 string css = "",
                 Action action = null)
        {
            return hb.Append(
                tag: "td",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "td",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder H(
            this HtmlBuilder hb,
            int number,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "h" + number,
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder H(
            this HtmlBuilder hb,
            int number,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "h" + number,
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Section(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "section",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Section(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "section",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Div(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "div",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Div(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "div",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder P(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "p",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder P(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "p",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Label(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "label",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Label(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "label",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Input(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "input",
                    closeLevel: 1,
                    attributes: new HtmlAttributes().Id_Css(id, css))
                : hb;
        }

        public static HtmlBuilder Input(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "input",
                    closeLevel: 1,
                    attributes: attributes)
                : hb;
        }

        public static HtmlBuilder TextArea(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            string placeholder = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "textarea",
                    attributes: new HtmlAttributes()
                        .Id_Css(id, css)
                        .Placeholder(placeholder),
                    action: action)
                : hb;
        }

        public static HtmlBuilder TextArea(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "textarea",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder TextArea(
            this HtmlBuilder hb,
            string id,
            string text = "",
            string controlCss = "",
            string placeholder = "",
            bool _using = true)
        {
            return _using
                ? hb.TextArea(
                    id: id,
                    css: CssClasses.Get("control-textarea", controlCss),
                    placeholder: placeholder,
                    action: () => hb.Text(text: text))
                : hb;
        }

        public static HtmlBuilder Button(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "button",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Button(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "button",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Select(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "select",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Select(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "select",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Option(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "option",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Option(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "option",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Span(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "span",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Span(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "span",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Em(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "em",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Em(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "em",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Ul(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "ul",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Ul(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "ul",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Ol(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "ol",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Ol(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "ol",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Li(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "li",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Li(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "li",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder A(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            string href = "",
            string text = "",
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "a",
                    attributes: new HtmlAttributes()
                        .Id_Css(id, css)
                        .Href(href),
                    action: () => hb
                        .Text(text: text))
                : hb;
        }

        public static HtmlBuilder A(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "a",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Form(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "form",
                    id: id,
                    css: css,
                    action: action)
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
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Time(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "time",
                    id: id,
                    css: css,
                    action: action)
                : hb;
        }

        public static HtmlBuilder Time(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "time",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder FieldSet(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "fieldset",
                    attributes: attributes,
                    action: action)
                : hb;
        }

        public static HtmlBuilder FieldSet(
            this HtmlBuilder hb,
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "fieldset",
                    action: action)
                : hb;
        }

        public static HtmlBuilder FieldSet(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            string legendText = "",
            bool _using = true,
            Action action = null)
        {
            return _using
                ? hb.Append(
                    tag: "fieldset",
                    id: id,
                    css: CssClasses.Get("fieldset cf", css),
                    action: () =>
                    {
                        if (legendText != string.Empty)
                        {
                            hb.Append(
                                tag: "legend",
                                css: "legend",
                                action: () => hb.Text(legendText));
                        }
                        if (action != null) action();
                    })
                : hb;
        }

        public static HtmlBuilder Img(
            this HtmlBuilder hb,
            string src = "",
            string css = "",
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "img",
                    attributes: new HtmlAttributes()
                        .Src(src)
                        .Class(css),
                    closeLevel: 1)
                : hb;
        }

        public static HtmlBuilder Script(
            this HtmlBuilder hb,
            string src = "",
            string script = "",
            bool _using = true)
        {
            return _using
                ? hb.Append(
                    tag: "script",
                    attributes: new HtmlAttributes().Src(src),
                    action: () => hb
                        .Raw(text: script))
                : hb;
        }
    }
}