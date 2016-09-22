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

        public HtmlAttributes Add(string name, string value, bool _using = true)
        {
            if (value != null && _using)
            {
                base.Add(name);
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Add(Dictionary<string, string> attributes, bool _using = true)
        {
            if (attributes != null && _using)
            {
                attributes.ForEach(attribute =>
                {
                    base.Add(attribute.Key);
                    base.Add(attribute.Value);
                });
            }
            return this;
        }

        public HtmlAttributes Id(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("id");
                base.Add(value);
                base.Add("name");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Class(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("class");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Style(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("style");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Type(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("type");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Src(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("src");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Value(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("value");
                base.Add(HttpUtility.HtmlEncode(value));
            }
            return this;
        }

        public HtmlAttributes RawValue(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("value");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataMax(decimal value, bool _using = true)
        {
            if (_using)
            {
                base.Add("data-max");
                base.Add(value.ToString());
            }
            return this;
        }

        public HtmlAttributes DataMin(decimal value, bool _using = true)
        {
            if (_using)
            {
                base.Add("data-min");
                base.Add(value.ToString());
            }
            return this;
        }

        public HtmlAttributes DataStep(decimal value, bool _using = true)
        {
            if (_using)
            {
                base.Add("data-step");
                base.Add(value.ToString());
            }
            return this;
        }

        public HtmlAttributes Checked(bool value, bool _using = true)
        {
            if (value && _using)
            {
                base.Add("checked");
            }
            return this;
        }

        public HtmlAttributes Selected(bool value, bool _using = true)
        {
            if (value && _using)
            {
                base.Add("selected");
            }
            return this;
        }

        public HtmlAttributes Multiple(bool value, bool _using = true)
        {
            if (value && _using)
            {
                base.Add("multiple");
                base.Add("multiple");
            }
            return this;
        }

        public HtmlAttributes Action(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("action");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes For(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("for");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Href(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("href");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Title(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("title");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Placeholder(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("placeholder");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes AccessKey(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("accesskey");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes OnClick(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("onclick");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes OnDblClick(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("ondblclick");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes OnChange(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("onchange");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Disabled(bool value = false, bool _using = true)
        {
            if (value && _using)
            {
                base.Add("disabled");
                base.Add("disabled");
            }
            return this;
        }

        public HtmlAttributes Colspan(int value, bool _using = true)
        {
            if (value != 0 && _using)
            {
                base.Add("colspan");
                base.Add(value.ToString());
            }
            return this;
        }

        public HtmlAttributes DateTime(DateTime value, bool _using = true)
        {
            if (value.InRange() && _using)
            {
                base.Add("datetime");
                base.Add(value.ToString("s"));
            }
            return this;
        }

        public HtmlAttributes DataId(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("data-id");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataType(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("data-type");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataMethod(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("data-method");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataAction(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("data-action");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataIcon(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("data-icon");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataSelector(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("data-selector");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataValue(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("data-value");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataVer(int value, bool _using = true)
        {
            if (_using)
            {
                base.Add("data-ver");
                base.Add(value.ToString());
            }
            return this;
        }

        public HtmlAttributes DataLatest(int value, bool _using = true)
        {
            if (_using)
            {
                base.Add("data-latest");
                base.Add(value.ToString());
            }
            return this;
        }

        public HtmlAttributes DataConfirm(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("data-confirm");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataClass(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("data-class");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataStyle(string value, bool _using = true)
        {
            if (!value.IsNullOrEmpty() && _using)
            {
                base.Add("data-style");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataWidth(int value, bool _using = true)
        {
            if (_using)
            {
                base.Add("data-width");
                base.Add(value.ToString());
            }
            return this;
        }
    }
}