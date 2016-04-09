using Implem.Pleasanter.Libraries.Styles;
using System;
namespace Implem.Pleasanter.Libraries.Views
{
    public static class HtmlTags
    {
        public static HtmlBuilder Header(
            this HtmlBuilder hb,
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "header",
                css: css,
                action: action);
        }

        public static HtmlBuilder Footer(
            this HtmlBuilder hb,
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "footer",
                css: css,
                action: action);
        }

        public static HtmlBuilder Article(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "article",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Article(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            Action action = null)
        {
            return hb.Append(
                tag: "article",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Article(
            this HtmlBuilder hb,
            Action action = null)
        {
            return hb.Append(
                tag: "article",
                action: action);
        }

        public static HtmlBuilder Nav(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "nav",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Nav(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            Action action = null)
        {
            return hb.Append(
                tag: "nav",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Nav(
            this HtmlBuilder hb,
            Action action = null)
        {
            return hb.Append(
                tag: "nav",
                action: action);
        }

        public static HtmlBuilder Table(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "table",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Table(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            Action action = null)
        {
            return hb.Append(
                tag: "table",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Table(
            this HtmlBuilder hb,
            Action action = null)
        {
            return hb.Append(
                tag: "table",
                action: action);
        }

        public static HtmlBuilder Caption(
            this HtmlBuilder hb,
            string caption)
        {
            return hb.Append(
                tag: "caption",
                action: () => hb
                    .Text(text: caption));
        }

        public static HtmlBuilder Th(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "th",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Th(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            Action action = null)
        {
            return hb.Append(
                tag: "th",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Th(
            this HtmlBuilder hb,
            Action action = null)
        {
            return hb.Append(
                tag: "th",
                action: action);
        }

        public static HtmlBuilder Tr(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "tr",
                id: id,
                css: css,
                action: action);
        }
       
        public static HtmlBuilder Tr(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            Action action = null)
        {
            return hb.Append(
                tag: "tr",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Tr(
            this HtmlBuilder hb,
            Action action = null)
        {
            return hb.Append(
                tag: "tr",
                action: action);
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
            Action action = null)
        {
            return hb.Append(
                tag: "td",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Td(
            this HtmlBuilder hb,
            Action action = null)
        {
            return hb.Append(
                tag: "td",
                action: action);
        }

        public static HtmlBuilder H(
            this HtmlBuilder hb,
            int number,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "h" + number,
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder H(
            this HtmlBuilder hb,
            int number,
            HtmlAttributes attributes,
            Action action = null)
        {
            return hb.Append(
                tag: "h" + number,
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder H(
            this HtmlBuilder hb,
            int number,
            Action action = null)
        {
            return hb.Append(
                tag: "h" + number,
                action: action);
        }

        public static HtmlBuilder Section(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "section",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Section(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            Action action = null)
        {
            return hb.Append(
                tag: "section",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Section(
            this HtmlBuilder hb,
            Action action = null)
        {
            return hb.Append(
                tag: "section",
                action: action);
        }

        public static HtmlBuilder Div(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "div",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Div(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            Action action = null)
        {
            return hb.Append(
                tag: "div",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Div(
            this HtmlBuilder hb,
            Action action = null)
        {
            return hb.Append(
                tag: "div",
                action: action);
        }

        public static HtmlBuilder P(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "p",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder P(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null,
            Action action = null)
        {
            return hb.Append(
                tag: "p",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder P(
            this HtmlBuilder hb,
            Action action = null)
        {
            return hb.Append(
                tag: "p",
                action: action);
        }

        public static HtmlBuilder Label(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "label",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Label(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null,
            Action action = null)
        {
            return hb.Append(
                tag: "label",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Label(
            this HtmlBuilder hb,
            Action action = null)
        {
            return hb.Append(
                tag: "label",
                action: action);
        }

        public static HtmlBuilder Input(
            this HtmlBuilder hb,
            string id = "",
            string css = "")
        {
            return hb.Append(
                tag: "input",
                closeLevel: 1,
                attributes: Html.Attributes().Id_Css(id, css));
        }

        public static HtmlBuilder Input(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null)
        {
            return hb.Append(
                tag: "input",
                closeLevel: 1,
                attributes: attributes);
        }

        public static HtmlBuilder TextArea(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            string placeholder = "",
            Action action = null)
        {
            return hb.Append(
                tag: "textarea",
                attributes: Html.Attributes()
                    .Id_Css(id, css)
                    .Placeholder(placeholder),
                action: action);
        }

        public static HtmlBuilder TextArea(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null,
            Action action = null)
        {
            return hb.Append(
                tag: "textarea",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder TextArea(
            this HtmlBuilder hb,
            string id,
            string text = "",
            string controlCss = "",
            string placeholder = "")
        {
            return hb.TextArea(
                id: id,
                css: CssClasses.Get("control-textarea", controlCss),
                placeholder: placeholder,
                action: () => hb.Text(text: text));
        }

        public static HtmlBuilder Button(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "button",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Button(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null,
            Action action = null)
        {
            return hb.Append(
                tag: "button",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Select(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "select",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Select(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null,
            Action action = null)
        {
            return hb.Append(
                tag: "select",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Option(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "option",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Option(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null,
            Action action = null)
        {
            return hb.Append(
                tag: "option",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Span(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "span",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Span(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null,
            Action action = null)
        {
            return hb.Append(
                tag: "span",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Span(
            this HtmlBuilder hb,
            Action action = null)
        {
            return hb.Append(
                tag: "span",
                action: action);
        }

        public static HtmlBuilder Em(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "em",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Em(
            this HtmlBuilder hb,
            HtmlAttributes attributes,
            Action action = null)
        {
            return hb.Append(
                tag: "em",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Em(
            this HtmlBuilder hb,
            Action action = null)
        {
            return hb.Append(
                tag: "em",
                action: action);
        }

        public static HtmlBuilder Ul(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "ul",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Ul(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null,
            Action action = null)
        {
            return hb.Append(
                tag: "ul",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Ul(
            this HtmlBuilder hb,
            Action action = null)
        {
            return hb.Append(
                tag: "ul",
                action: action);
        }

        public static HtmlBuilder Ol(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "ol",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Ol(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null,
            Action action = null)
        {
            return hb.Append(
                tag: "ol",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Ol(
            this HtmlBuilder hb,
            Action action = null)
        {
            return hb.Append(
                tag: "ol",
                action: action);
        }

        public static HtmlBuilder Li(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "li",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Li(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null,
            Action action = null)
        {
            return hb.Append(
                tag: "li",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Li(
            this HtmlBuilder hb,
            Action action = null)
        {
            return hb.Append(
                tag: "li",
                action: action);
        }

        public static HtmlBuilder A(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            string href = "",
            string text = "")
        {
            return hb.Append(
                tag: "a",
                attributes: Html.Attributes()
                    .Id_Css(id, css)
                    .Href(href),
                action: () => hb
                    .Text(text: text));
        }

        public static HtmlBuilder A(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null,
            Action action = null)
        {
            return hb.Append(
                tag: "a",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Form(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "form",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Form(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null,
            Action action = null)
        {
            return hb.Append(
                tag: "form",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder Time(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            Action action = null)
        {
            return hb.Append(
                tag: "time",
                id: id,
                css: css,
                action: action);
        }

        public static HtmlBuilder Time(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null,
            Action action = null)
        {
            return hb.Append(
                tag: "time",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder FieldSet(
            this HtmlBuilder hb,
            HtmlAttributes attributes = null,
            Action action = null)
        {
            return hb.Append(
                tag: "fieldset",
                attributes: attributes,
                action: action);
        }

        public static HtmlBuilder FieldSet(
            this HtmlBuilder hb,
            Action action = null)
        {
            return hb.Append(
                tag: "fieldset",
                action: action);
        }

        public static HtmlBuilder FieldSet(
            this HtmlBuilder hb,
            string id = "",
            string css = "",
            string legendText = "",
            Action action = null)
        {
            return hb.Append(
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
                    action();
                });
        }

        public static HtmlBuilder Img(
            this HtmlBuilder hb,
            string src = "",
            string css = "")
        {
            return hb.Append(
                tag: "img",
                attributes: Html.Attributes()
                    .Src(src)
                    .Class(css),
                closeLevel: 1);
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
                    attributes: Html.Attributes().Src(src),
                    action: () => hb
                        .Raw(text: script))
                : hb;
        }
    }
}