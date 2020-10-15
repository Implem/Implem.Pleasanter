using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace Implem.Pleasanter.Libraries.Html
{
    public class HtmlBuilder
    {
        public HtmlElement Top;
        public HtmlElement Current;

        public HtmlBuilder Append(string tag, int closeLevel = 0, HtmlAttributes attributes = null)
        {
            var element = new HtmlElement
            {
                Tag = tag
            };
            AppendAttributes(element, attributes);
            if (Top == null)
            {
                (Top, Current) = (element, element);
            }
            else
            {
                Current.Children.Add(element);
                element.Parent = Current;
            }
            switch (closeLevel)
            {
                case var level when level <= 0:
                    Current = element;
                    break;
                case var level when level == 1:
                    element.HasCloseTag = false;
                    break;
                case var level when level >= 2:
                    AppendCloseTagCollection(closeLevel - 1);
                    break;
            }
            return this;
        }

        private void AppendAttributes(HtmlElement element, HtmlAttributes attributes)
        {
            if (attributes == null)
            {
                return;
            }
            for (var i = 0; i < attributes.Count; i += 2)
            {
                if ((i + 1) < attributes.Count)
                {
                    element.Attributes.Add((attributes[i], attributes[i + 1], false));
                }
                else
                {
                    element.Attributes.Add((attributes[i], null, true));
                }
            }
        }

        public HtmlBuilder Append(string tag, Action action)
        {
            return Append(
                tag: tag,
                id: null,
                css: null,
                attributes: null,
                action: action);
        }

        public HtmlBuilder Append(string tag, HtmlAttributes attributes, Action action)
        {
            return Append(
                tag: tag,
                id: null,
                css: null,
                attributes: attributes,
                action: action);
        }

        public HtmlBuilder Append(
            string tag,
            string id,
            string css,
            HtmlAttributes attributes,
            Action action)
        {
            Append(tag: tag, attributes: (attributes ?? new HtmlAttributes())
                .Id(id)
                .Class(css));
            action?.Invoke();
            AppendClose();
            return this;
        }

        public HtmlBuilder Text(string text)
        {
            Current.Children.Add(new HtmlElement
            {
                Text = text
            });
            return this;
        }

        public HtmlBuilder Raw(string text)
        {
            Current.Children.Add(new HtmlElement
            {
                RawText = text
            });
            return this;
        }

        public HtmlBuilder AppendClose(int closeLevel = 1)
        {
            AppendCloseTagCollection(closeLevel);
            return this;
        }

        public override string ToString()
        {
            return Top?.ToHtml().ToString();
        }

        private void AppendCloseTagCollection(int closeLevel = -1)
        {
            if (Current?.Parent == null)
            {
                return;
            }
            if (closeLevel == -1)
            {
                Current = Top;
                return;
            }
            for (int i = 0; i < closeLevel; ++i)
            {
                if (Current.Parent != null)
                {
                    Current = Current.Parent;
                }
            }
        }

        public class HtmlElement
        {
            public string Tag;
            public string Id;
            public string Css;
            public string Text;
            public string RawText;
            public List<(string key, string value, bool noValue)> Attributes = new List<(string, string, bool)>();
            public List<HtmlElement> Children = new List<HtmlElement>();
            public HtmlElement Parent;
            public bool HasCloseTag = true;

            public StringBuilder ToHtml(StringBuilder builder = null)
            {
                builder = builder ?? new StringBuilder();
                if (string.IsNullOrWhiteSpace(Tag))
                {
                    return builder.Append(RawText).Append(HttpUtility.HtmlEncode(Text));
                }
                builder.Append("<").Append(Tag);
                if (!string.IsNullOrWhiteSpace(Css))
                {
                    builder.Append(" class=\"").Append(Css).Append("\"");
                }
                foreach (var (key, value, noValue) in Attributes)
                {
                    if (noValue)
                    {
                        builder.Append(" ").Append(key);
                    }
                    else
                    {
                        builder.Append(" ").Append(key).Append("=\"").Append(value).Append("\"");
                    }
                }
                if (!Children.Any() && !HasCloseTag)
                {
                    return builder.Append(" />");
                }
                builder.Append(">");
                foreach (var child in Children)
                {
                    child.ToHtml(builder);
                }
                return builder.Append("</").Append(Tag).Append(">");
            }

            public override string ToString()
            {
                return $"{Tag} {Id} {Text} {RawText}".Replace("  ", "").Trim();
            }
        }
    }
}
