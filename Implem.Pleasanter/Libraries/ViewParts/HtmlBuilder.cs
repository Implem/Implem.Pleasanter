using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace Implem.Pleasanter.Libraries.ViewParts
{
    public class HtmlBuilder
    {
        private StringBuilder html = new StringBuilder();
        private List<string> closeTagCollection = new List<string>();

        private enum AttributePropertyTypes : int
        {
            Name = 0,
            Value = 1
        }

        public HtmlBuilder Append(string tag, int closeLevel = 0, HtmlAttributes attributes = null)
        {
            html.Append("<", tag);
            AppendAttributes(attributes);
            if (closeLevel > 0)
            {
                html.Append(" />");
                if (closeLevel >= 2)
                {
                    AppendCloseTagCollection(closeLevel - 1);
                }
            }
            else
            {
                html.Append(">");
                closeTagCollection.Insert(0, "</" + tag + ">");
            }
            return this;
        }

        private void AppendAttributes(HtmlAttributes attributes)
        {
            attributes?
                .Select((o, i) => new { Value = o, PropertyType = (AttributePropertyTypes)(i % 2) })
                .ForEach(htmlAttribute =>
                {
                    if (htmlAttribute.PropertyType == AttributePropertyTypes.Name)
                    {
                        html.Append(" " + htmlAttribute.Value);
                    }
                    else
                    {
                        html.Append("=\"", htmlAttribute.Value, "\"");
                    }
                });
        }

        public HtmlBuilder Append(string tag, HtmlAttributes attributes, Action action)
        {
            Append(tag: tag, attributes: attributes);
            if (action != null) action();
            AppendClose();
            return this;
        }

        public HtmlBuilder Append(string tag, Action action)
        {
            Append(tag);
            if (action != null) action();
            AppendClose();
            return this;
        }

        public HtmlBuilder Append(string tag, string id, string css, Action action)
        {
            Append(tag: tag, attributes: ViewParts.Html.Attributes()
                .Id(id)
                .Class(css));
            if (action != null) action();
            AppendClose();
            return this;
        }

        public HtmlBuilder Append(string tag, string css, Action action)
        {
            Append(tag: tag, attributes: ViewParts.Html.Attributes().Class(css));
            if (action != null) action();
            AppendClose();
            return this;
        }

        public HtmlBuilder Text(string text, int closeLevel = 0)
        {
            html.Append(HttpUtility.HtmlEncode(text));
            AppendCloseTagCollection(closeLevel);
            return this;
        }

        public HtmlBuilder Raw(string text)
        {
            html.Append(text);
            return this;
        }

        public HtmlBuilder AppendClose(int closeLevel = 1)
        {
            AppendCloseTagCollection(closeLevel);
            return this;
        }

        public override string ToString()
        {
            AppendCloseTagCollection();
            return html.ToString();
        }

        public ResponseCollection Html(string target)
        {
            return new ResponseCollection().Html(target, this);
        }

        private void AppendCloseTagCollection(int closeLevel = -1)
        {
            if (closeTagCollection.Count() > 0)
            {
                if (closeLevel == -1)
                {
                    html.Append(closeTagCollection.Join(string.Empty));
                    closeTagCollection.Clear();
                }
                else
                {
                    html.Append(closeTagCollection.Take(closeLevel).Join(string.Empty));
                    closeTagCollection = closeTagCollection.Skip(closeLevel).ToList<string>();
                }
            }
        }
    }
}
