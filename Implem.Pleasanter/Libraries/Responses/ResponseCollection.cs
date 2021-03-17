﻿using Implem.Libraries.Utilities;
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

        public ResponseCollection Add(
            string method,
            string target = null,
            object value = null,
            string options = null)
        {
            Add(new Response(
                method: method,
                target: target,
                value: value != null
                    ? value
                    : string.Empty,
                options: options));
            return this;
        }

        public ResponseCollection Html(
            string target,
            HtmlBuilder value,
            string options = null,
            bool _using = true)
        {
            return _using
                ? Html(
                    target: target,
                    value: value.ToString(),
                    options: options)
                : this;
        }

        public ResponseCollection Html(
            string target,
            object value,
            string options = null,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "Html",
                    target: target,
                    value: value,
                    options: options)
                : this;
        }

        public ResponseCollection ReplaceAll(
            string target,
            HtmlBuilder value,
            string options = null,
            bool _using = true)
        {
            return _using
                ? ReplaceAll(
                    target: target,
                    value: value.ToString(),
                    options: options)
                : this;
        }

        public ResponseCollection ReplaceAll(
            string target,
            object value,
            string options = null,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "ReplaceAll",
                    target: target,
                    value: value,
                    options: options)
                : this;
        }

        public ResponseCollection Message(
            Message message,
            string target = null,
            bool _using = true)
        {
            if (message != null && _using)
            {
                Add(
                    method: "Message",
                    target: target,
                    value: message.ToJson());
            }
            return this;
        }

        public ResponseCollection Href(
            string url,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "Href",
                    target: null,
                    value: url)
                : this;
        }

        public ResponseCollection PushState(
            string state,
            string url,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "PushState",
                    target: state,
                    value: url)
                : this;
        }

        public ResponseCollection SetData(
            string target,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "SetData",
                    target: target)
                : this;
        }

        public ResponseCollection SetFormData(
            string target,
            object value,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "SetFormData",
                    target: target,
                    value: value)
                : this;
        }

        public ResponseCollection SetMemory(
            string target,
            object value,
            bool _using = true)
        {
            return _using ? Add(method: "SetMemory", target: target, value: value) : this;
        }

        public ResponseCollection Append(
            string target,
            HtmlBuilder value,
            bool _using = true)
        {
            return _using ? Add(
                method: "Append",
                target: target,
                value: value.ToString()) : this;
        }

        public ResponseCollection Prepend(
            string target,
            HtmlBuilder value,
            bool _using = true)
        {
            return _using ? Add(
                method: "Prepend",
                target: target,
                value: value.ToString()) : this;
        }

        public ResponseCollection After(
            string target,
            HtmlBuilder value,
            bool _using = true)
        {
            return _using ? Add(
                method: "After",
                target: target,
                value: value.ToString()) : this;
        }

        public ResponseCollection Before(
            string target,
            HtmlBuilder value,
            bool _using = true)
        {
            return _using ? Add(
                method: "Before",
                target: target,
                value: value.ToString()) : this;
        }

        public ResponseCollection InsertText(
            string target,
            string value,
            bool _using = true)
        {
            return _using ? Add(
                method: "InsertText",
                target: target,
                value: value) : this;
        }

        public ResponseCollection Remove(
            string target,
            bool _using = true)
        {
            return _using ? Add(
                method: "Remove",
                target: target,
                value: string.Empty) : this;
        }

        public ResponseCollection Attr(
            string target,
            string name,
            string value,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "Attr",
                    target: target,
                    value: new { Name = name, Value = value }.ToJson())
                : this;
        }

        public ResponseCollection RemoveAttr(
            string target,
            string name,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "RemoveAttr",
                    target: target,
                    value: name)
                : this;
        }

        public ResponseCollection Css(
            string target,
            string name,
            string value,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "Css",
                    target: target,
                    value: new { Name = name, Value = value }.ToJson())
                : this;
        }

        public ResponseCollection Focus(
            string target,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "Focus",
                    target: target)
                : this;
        }

        public ResponseCollection Val(
            string target,
            object value,
            string options = null,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "SetValue",
                    target: target,
                    value: value,
                    options: options)
                : this;
        }

        public ResponseCollection ValAndFormData(
            string target,
            object value,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "SetValueAndFormData",
                    target: target,
                    value: value)
                : this;
        }

        public ResponseCollection ClearFormData(
            string target = null,
            string type = null,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "ClearFormData",
                    target: target,
                    value: type)
                : this;
        }

        public ResponseCollection CloseDialog(
            string target = null,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "CloseDialog",
                    target: target)
                : this;
        }

        public ResponseCollection Paging(
            string target,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "Paging",
                    target: target)
                : this;
        }

        public ResponseCollection Toggle(
            string name,
            bool value,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "Toggle",
                    target: name,
                    value: value.ToOneOrZeroString())
                : this;
        }

        public ResponseCollection Trigger(
            string name,
            string _event,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "Trigger",
                    target: name,
                    value: _event)
                : this;
        }

        public ResponseCollection Invoke(
            string methodName,
            string args = null,
            bool _using = true)
        {
            return !methodName.IsNullOrEmpty() && _using
                ? Add(
                    method: "Invoke",
                    target: methodName,
                    value: args)
                : this;
        }

        public ResponseCollection Events(
            string target,
            bool _using = true)
        {
            return !target.IsNullOrEmpty() && _using
                ? Add(
                    method: "Events",
                    target: target)
                : this;
        }

        public ResponseCollection WindowScrollTop(
            int value = 0,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "WindowScrollTop",
                    target: null,
                    value: value)
                : this;
        }

        public ResponseCollection ScrollTop(
            string target,
            int value = 0,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "ScrollTop",
                    target: target,
                    value: value)
                : this;
        }

        public ResponseCollection LoadScroll(bool _using = true)
        {
            return _using
                ? Add(
                    method: "LoadScroll",
                    target: null,
                    value: null)
                : this;
        }

        public ResponseCollection FocusMainForm(bool _using = true)
        {
            return _using
                ? Add(method: "FocusMainForm")
                : this;
        }

        public ResponseCollection Disabled(
            string key,
            bool value = true,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "Disabled",
                    target: key,
                    value: value)
                : this;
        }

        public ResponseCollection Log(
            string value,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "Log",
                    target: null,
                    value: value)
                : this;
        }

        public ResponseCollection Response(
            string key,
            string value,
            bool _using = true)
        {
            return _using
                ? Add(
                    method: "Response",
                    target: key,
                    value: value)
                : this;
        }
    }
}