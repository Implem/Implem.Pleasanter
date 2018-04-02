using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Responses
{
    [Serializable]
    public class ResponseCollection : List<Response>
    {
        public ResponseCollection()
        {
        }

        public ResponseCollection Add(string method, string target = null, object value = null)
        {
            Add(new Response(method, target, value != null ? value : string.Empty));
            return this;
        }

        public ResponseCollection Html(string target, HtmlBuilder value, bool _using = true)
        {
            return _using ? Html(target, value.ToString()) : this;
        }

        public ResponseCollection Html(string target, object value, bool _using = true)
        {
            return _using ? Add("Html", target, value) : this;
        }

        public ResponseCollection ReplaceAll(string target, HtmlBuilder value, bool _using = true)
        {
            return _using ? ReplaceAll(target, value.ToString()) : this;
        }

        public ResponseCollection ReplaceAll(string target, object value, bool _using = true)
        {
            return _using ? Add("ReplaceAll", target, value) : this;
        }

        public ResponseCollection Message(Message message, string target = null, bool _using = true)
        {
            if (message != null && _using)
            {
                Add("Message", target, message.ToJson());
            }
            return this;
        }

        public ResponseCollection Href(string url, bool _using = true)
        {
            return _using ? Add("Href", null, url) : this;
        }

        public ResponseCollection PushState(string state, string url, bool _using = true)
        {
            return _using ? Add("PushState", state, url) : this;
        }

        public ResponseCollection SetData(string target, bool _using = true)
        {
            return _using ? Add("SetData", target) : this;
        }

        public ResponseCollection SetFormData(string target, object value, bool _using = true)
        {
            return _using ? Add("SetFormData", target, value) : this;
        }

        public ResponseCollection SetMemory(string target, object value, bool _using = true)
        {
            return _using ? Add("SetMemory", target, value) : this;
        }

        public ResponseCollection Append(string target, HtmlBuilder value, bool _using = true)
        {
            return _using ? Add("Append", target, value.ToString()) : this;
        }

        public ResponseCollection Prepend(string target, HtmlBuilder value, bool _using = true)
        {
            return _using ? Add("Prepend", target, value.ToString()) : this;
        }

        public ResponseCollection After(string target, HtmlBuilder value, bool _using = true)
        {
            return _using ? Add("After", target, value.ToString()) : this;
        }

        public ResponseCollection Before(string target, HtmlBuilder value, bool _using = true)
        {
            return _using ? Add("Before", target, value.ToString()) : this;
        }

        public ResponseCollection InsertText(string target, string value, bool _using = true)
        {
            return _using ? Add("InsertText", target, value) : this;
        }

        public ResponseCollection Remove(string target, bool _using = true)
        {
            return _using ? Add("Remove", target, string.Empty) : this;
        }

        public ResponseCollection Attr(string target, string name, string value, bool _using = true)
        {
            return _using
                ? Add("Attr", target, new { Name = name, Value = value }.ToJson())
                : this;
        }

        public ResponseCollection RemoveAttr(string target, string name, bool _using = true)
        {
            return _using ? Add("RemoveAttr", target, name) : this;
        }

        public ResponseCollection Focus(string target, bool _using = true)
        {
            return _using ? Add("Focus", target) : this;
        }

        public ResponseCollection Val(string target, object value, bool _using = true)
        {
            return _using ? Add("SetValue", target, value) : this;
        }

        public ResponseCollection ValAndFormData(string target, object value, bool _using = true)
        {
            return _using ? Add("SetValueAndFormData", target, value) : this;
        }

        public ResponseCollection ClearFormData(
            string target = null, string type = null, bool _using = true)
        {
            return _using ? Add("ClearFormData", target, type) : this;
        }

        public ResponseCollection CloseDialog(string target = null, bool _using = true)
        {
            return _using ? Add("CloseDialog", target) : this;
        }

        public ResponseCollection Paging(string target, bool _using = true)
        {
            return _using ? Add("Paging", target) : this;
        }

        public ResponseCollection Toggle(string name, bool value, bool _using = true)
        {
            return _using ? Add("Toggle", name, value.ToOneOrZeroString()) : this;
        }

        public ResponseCollection Trigger(string name, string _event, bool _using = true)
        {
            return _using ? Add("Trigger", name, _event) : this;
        }

        public ResponseCollection Invoke(string methodName, bool _using = true)
        {
            return !methodName.IsNullOrEmpty() && _using
                ? Add("Invoke", methodName)
                : this;
        }

        public ResponseCollection WindowScrollTop(int value = 0, bool _using = true)
        {
            return _using ? Add("WindowScrollTop", null, value) : this;
        }

        public ResponseCollection ScrollTop(string target, int value = 0, bool _using = true)
        {
            return _using ? Add("ScrollTop", target, value) : this;
        }

        public ResponseCollection LoadScroll(bool _using = true)
        {
            return _using ? Add("LoadScroll", null, null) : this;
        }

        public ResponseCollection FocusMainForm(bool _using = true)
        {
            return _using ? Add("FocusMainForm") : this;
        }

        public ResponseCollection Disabled(string key, bool value = true, bool _using = true)
        {
            return _using ? Add("Disabled", key, value) : this;
        }
    }
}