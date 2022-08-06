﻿using Implem.Libraries.Utilities;
using Implem.PleasanterTest.Models;

namespace Implem.PleasanterTest.Utilities
{
    public static class JsonData
    {
        public static JsonTest Html(string target, string selector)
        {
            return HtmlTests(
                method: "Html",
                target: target,
                selector: selector);
        }

        public static JsonTest ReplaceAll(string target, string selector)
        {
            return HtmlTests(
                method: "ReplaceAll",
                target: target,
                selector: selector);
        }

        public static JsonTest Append(string target, string selector)
        {
            return HtmlTests(
                method: "Append",
                target: target,
                selector: selector);
        }

        public static JsonTest ExistsOne(
            string method,
            string target = null)
        {
            return new JsonTest()
            {
                Type = JsonTest.Types.ExistsOne,
                Method = method,
                Target = target
            };
        }

        public static JsonTest HtmlTests(
            string method,
            string target,
            string selector)
        {
            return new JsonTest()
            {
                Type = JsonTest.Types.Html,
                Method = method,
                Target = target,
                HtmlTests = HtmlData.ExistsOne(selector: selector).ToSingleList()
            };
        }

        public static JsonTest Message(string message)
        {
            return new JsonTest()
            {
                Type = JsonTest.Types.Message,
                Method = "Message",
                Value = message
            };
        }
    }
}
