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

        public HtmlAttributes Id(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("id");
                base.Add(value);
                base.Add("name");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Class(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("class");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Id_Css(string id, string css)
        {
            Id(id);
            Class(css);
            return this;
        }

        public HtmlAttributes Style(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("style");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Type(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("type");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Src(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("src");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Value(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("value");
                base.Add(HttpUtility.HtmlEncode(value));
            }
            return this;
        }

        public HtmlAttributes DataMax(decimal value)
        {
            base.Add("data-max");
            base.Add(value.ToString());
            return this;
        }

        public HtmlAttributes DataMin(decimal value)
        {
            base.Add("data-min");
            base.Add(value.ToString());
            return this;
        }

        public HtmlAttributes DataStep(decimal value)
        {
            base.Add("data-step");
            base.Add(value.ToString());
            return this;
        }

        public HtmlAttributes Checked(bool value)
        {
            if (value)
            {
                base.Add("checked");
            }
            return this;
        }

        public HtmlAttributes Selected(bool value)
        {
            if (value)
            {
                base.Add("selected");
            }
            return this;
        }

        public HtmlAttributes Action(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("action");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes For(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("for");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Href(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("href");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Title(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("title");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Placeholder(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("placeholder");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes AccessKey(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("accesskey");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes OnClick(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("onclick");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes OnDblClick(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("ondblclick");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes OnChange(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("onchange");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes Disabled(bool value = false)
        {
            if (value)
            {
                base.Add("disabled");
                base.Add("disabled");
            }
            return this;
        }

        public HtmlAttributes Colspan(int value)
        {
            if (value != 0)
            {
                base.Add("colspan");
                base.Add(value.ToString());
            }
            return this;
        }

        public HtmlAttributes DateTime(DateTime value)
        {
            if (value != null && value.NotZero())
            {
                base.Add("datetime");
                base.Add(value.ToString("s"));
            }
            return this;
        }

        public HtmlAttributes DataId(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("data-id");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataType(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("data-type");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataMethod(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("data-method");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataAction(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("data-action");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataSelector(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("data-selector");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataValue(string value)
        {
            if (!value.IsNullOrEmpty())
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

        public HtmlAttributes DataConfirm(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("data-confirm");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataClass(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("data-class");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataStyle(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                base.Add("data-style");
                base.Add(value);
            }
            return this;
        }

        public HtmlAttributes DataWidth(int value)
        {
            base.Add("data-width");
            base.Add(value.ToString());
            return this;
        }
    }
}