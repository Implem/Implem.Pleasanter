using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Implem.Pleasanter.Libraries.Html
{
    public class HtmlAttributes : List<string>
    {
        public HtmlAttributes()
        {
        }

        public new HtmlAttributes Add(string name)
        {
            return Add(name, null);
        }

        public HtmlAttributes Add(string name, string value, bool _using = true)
        {
            if (_using)
            {
                base.Add(name);
                if (value != null)
                {
                    base.Add(value);
                }
            }
            return this;
        }

        public HtmlAttributes Add(string name, int value, bool _using = true)
        {
            if (_using)
            {
                base.Add(name);
                if (value != -1)
                {
                    base.Add(value.ToString());
                }
            }
            return this;
        }

        public HtmlAttributes Add(Dictionary<string, string> attributes)
        {
            if (attributes != null)
            {
                attributes.ForEach(attribute =>
                {
                    base.Add(attribute.Key);
                    base.Add(attribute.Value);
                });
            }
            return this;
        }

        public HtmlAttributes Id(string id, int index = -1)
        {
            if (id != string.Empty)
            {
                if (index == -1)
                {
                    base.Add("id");
                    base.Add(id);
                }
                else
                {
                    base.Add("id");
                    base.Add(id + index);
                }
                base.Add("name");
                base.Add(id);
            }
            return this;
        }

        public HtmlAttributes Name(string name)
        {
            if (name != string.Empty)
            {
                base.Add("name");
                base.Add(name);
            }
            return this;
        }

        public HtmlAttributes Class(string css)
        {
            if (css != string.Empty)
            {
                base.Add("class");
                base.Add(css);
            }
            return this;
        }

        public HtmlAttributes Id_Css(string id, string css)
        {
            Id(id);
            Class(css);
            return this;
        }

        public HtmlAttributes Style(string style)
        {
            if (style != string.Empty)
            {
                base.Add("style");
                base.Add(style);
            }
            return this;
        }

        public HtmlAttributes Width(int width)
        {
            base.Add("width");
            base.Add(width.ToString());
            return this;
        }

        public HtmlAttributes Height(int height)
        {
            base.Add("height");
            base.Add(height.ToString());
            return this;
        }

        public HtmlAttributes Type(string type)
        {
            if (type != string.Empty)
            {
                base.Add("type");
                base.Add(type);
            }
            return this;
        }

        public HtmlAttributes Src(string src)
        {
            if (src != string.Empty)
            {
                base.Add("src");
                base.Add(src);
            }
            return this;
        }

        public HtmlAttributes Value(string value)
        {
            if (value != string.Empty)
            {
                base.Add("value");
                base.Add(HttpUtility.HtmlEncode(value));
            }
            return this;
        }

        public HtmlAttributes Max(decimal max)
        {
            base.Add("max");
            base.Add(max.ToString());
            return this;
        }

        public HtmlAttributes Min(decimal min)
        {
            base.Add("min");
            base.Add(min.ToString());
            return this;
        }

        public HtmlAttributes Step(decimal step)
        {
            base.Add("step");
            base.Add(step.ToString());
            return this;
        }

        public HtmlAttributes Checked(bool _checked)
        {
            if (_checked)
            {
                base.Add("checked");
            }
            return this;
        }

        public HtmlAttributes Selected(bool selected)
        {
            if (selected)
            {
                base.Add("selected");
            }
            return this;
        }

        public HtmlAttributes Action(string action)
        {
            if (action != string.Empty)
            {
                base.Add("action");
                base.Add(action);
            }
            return this;
        }

        public HtmlAttributes Enctype(string enctype)
        {
            if (enctype != string.Empty)
            {
                base.Add("enctype");
                base.Add(enctype);
            }
            return this;
        }

        public HtmlAttributes For(string _for)
        {
            if (_for != string.Empty)
            {
                base.Add("for");
                base.Add(_for);
            }
            return this;
        }

        public HtmlAttributes Href(string href)
        {
            if (href != string.Empty)
            {
                base.Add("href");
                base.Add(href);
            }
            return this;
        }

        public HtmlAttributes Title(string title)
        {
            if (title != string.Empty)
            {
                base.Add("title");
                base.Add(title);
            }
            return this;
        }

        public HtmlAttributes Placeholder(string placeholder)
        {
            if (placeholder != string.Empty)
            {
                base.Add("placeholder");
                base.Add(placeholder);
            }
            return this;
        }

        public HtmlAttributes AccessKey(string accessKey)
        {
            if (accessKey != string.Empty)
            {
                base.Add("accesskey");
                base.Add(accessKey);
            }
            return this;
        }

        public HtmlAttributes OnClick(string onClick)
        {
            if (onClick != string.Empty)
            {
                base.Add("onclick");
                base.Add(onClick);
            }
            return this;
        }

        public HtmlAttributes OnDblClick(string onDblClick)
        {
            if (onDblClick != string.Empty)
            {
                base.Add("ondblclick");
                base.Add(onDblClick);
            }
            return this;
        }

        public HtmlAttributes OnChange(string onChange)
        {
            if (onChange != string.Empty)
            {
                base.Add("onchange");
                base.Add(onChange);
            }
            return this;
        }

        public HtmlAttributes Disabled(bool disabled = false)
        {
            if (disabled)
            {
                base.Add("disabled");
                base.Add("disabled");
            }
            return this;
        }

        public HtmlAttributes Colspan(int colspan)
        {
            if (colspan != 0)
            {
                base.Add("colspan");
                base.Add(colspan.ToString());
            }
            return this;
        }

        public HtmlAttributes DateTime(DateTime dateTime)
        {
            if (dateTime != null && dateTime.NotZero())
            {
                base.Add("datetime");
                base.Add(dateTime.ToString("s"));
            }
            return this;
        }

        public HtmlAttributes DataId(string dataId)
        {
            if (dataId != string.Empty)
            {
                base.Add("data-id");
                base.Add(dataId);
            }
            return this;
        }

        public HtmlAttributes DataType(string dataType)
        {
            if (dataType != string.Empty)
            {
                base.Add("data-type");
                base.Add(dataType);
            }
            return this;
        }

        public HtmlAttributes DataMethod(string dataMethod)
        {
            if (dataMethod != string.Empty)
            {
                base.Add("data-method");
                base.Add(dataMethod);
            }
            return this;
        }

        public HtmlAttributes DataAction(string dataAction)
        {
            if (dataAction != string.Empty)
            {
                base.Add("data-action");
                base.Add(dataAction);
            }
            return this;
        }

        public HtmlAttributes DataSelector(string dataSelector)
        {
            if (dataSelector != string.Empty)
            {
                base.Add("data-selector");
                base.Add(dataSelector);
            }
            return this;
        }

        public HtmlAttributes DataValue(string dataValue)
        {
            if (dataValue != string.Empty)
            {
                base.Add("data-value");
                base.Add(dataValue);
            }
            return this;
        }

        public HtmlAttributes DataConfirm(string dataConfirm)
        {
            if (dataConfirm != string.Empty)
            {
                base.Add("data-confirm");
                base.Add(dataConfirm);
            }
            return this;
        }

        public HtmlAttributes DataClass(string dataClass)
        {
            if (dataClass != string.Empty)
            {
                base.Add("data-class");
                base.Add(dataClass);
            }
            return this;
        }

        public HtmlAttributes DataStyle(string dataStyle)
        {
            if (dataStyle != string.Empty)
            {
                base.Add("data-style");
                base.Add(dataStyle);
            }
            return this;
        }

        public HtmlAttributes DataWidth(int dataWidth)
        {
            base.Add("data-width");
            base.Add(dataWidth.ToString());
            return this;
        }
    }
}