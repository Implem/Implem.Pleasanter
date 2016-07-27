using Implem.Pleasanter.Libraries.Html;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Responses
{
    public class Response
    {
        public string Method;
        public string Target;
        public object Value;

        public Response(string method, string target, object value)
        {
            Method = method;
            Target = target;
            Value = value;
        }
    }

    [Serializable]
    public class ResponseCollection : List<Response>
    {
        public ResponseCollection()
        {
        }

        public ResponseCollection Add(string method, string target, object value = null)
        {
            Add(new Response(method, target, value != null ? value : string.Empty));
            return this;
        }

        public ResponseCollection Html(string target, HtmlBuilder value)
        {
            return Html(target, value.ToString());
        }

        public ResponseCollection Html(string target, object value)
        {
            return Add("Html", target, value);
        }

        public ResponseCollection ReplaceAll(string target, HtmlBuilder value, bool _using = true)
        {
            return _using
                ? ReplaceAll(target, value.ToString())
                : this;
        }

        public ResponseCollection ReplaceAll(string target, object value)
        {
            return Add("ReplaceAll", target, value);
        }

        public ResponseCollection Message(Message message)
        {
            if (message != null)
            {
                Add("Message", string.Empty, message.Html);
                Add("Status", string.Empty, message.Status);
            }
            return this;
        }

        public ResponseCollection Href(string url = null)
        {
            return Add("Href", string.Empty, url);
        }

        public ResponseCollection PushState(string state, string url, bool _using = true)
        {
            return _using
                ? Add("PushState", state, url)
                : this;
        }

        public ResponseCollection SetFormData(string target, object value)
        {
            return Add("SetFormData", target, value);
        }

        public ResponseCollection Append(string target, HtmlBuilder value)
        {
            return Add("Append", target, value.ToString());
        }

        public ResponseCollection Prepend(string target, HtmlBuilder value)
        {
            return Add("Prepend", target, value.ToString());
        }

        public ResponseCollection After(string target, HtmlBuilder value)
        {
            return Add("After", target, value.ToString());
        }

        public ResponseCollection Before(string target, HtmlBuilder value)
        {
            return Add("Before", target, value.ToString());
        }


        public ResponseCollection Remove(string target, bool _using = true)
        {
            return _using ? Add("Remove", target, string.Empty) : this;
        }

        public ResponseCollection Focus(string target = "")
        {
            return Add("Focus", target, string.Empty);
        }

        public ResponseCollection Val(string target, object value)
        {
            return Add("SetValue", target, value);
        }

        public ResponseCollection ValAndFormData(string target, object value)
        {
            return Add("SetValueAndFormData", target, value);
        }

        public ResponseCollection Markup()
        {
            return Add("Markup", string.Empty, string.Empty);
        }

        public ResponseCollection ClearFormData(
            string target = "", string type = "", bool _using = true)
        {
            return _using ? Add("ClearFormData", target, type) : this;
        }

        public ResponseCollection CloseDialog(string target = "", bool _using = true)
        {
            return _using ? Add("CloseDialog", string.Empty, string.Empty) : this;
        }

        public ResponseCollection Trigger(string name, string _event, bool _using = true)
        {
            return _using ? Add("Trigger", name, _event) : this;
        }

        public ResponseCollection Func(string methodName, bool _using = true)
        {
            return _using ? Add("Func", methodName) : this;
        }

        public ResponseCollection Validation(string tableName, bool _using = true)
        {
            return _using ? Add("Validation", tableName) : this;
        }

        public ResponseCollection WindowScrollTop(int value = 0, bool _using = true)
        {
            return _using ? Add("WindowScrollTop", string.Empty, value) : this;
        }

        public ResponseCollection FocusMainForm()
        {
            return Add("FocusMainForm", string.Empty);
        }

        public ResponseCollection Empty(string key)
        {
            return Add("Empty", key, string.Empty);
        }

        public ResponseCollection Disabled(string key, bool value = true)
        {
            return Add("Disabled", key, value);
        }
    }
}