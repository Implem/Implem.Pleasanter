using Implem.Pleasanter.Libraries.Responses;
using Implem.PleasanterTest.Models;
using System.Collections.Generic;

namespace Implem.PleasanterTest.Utilities
{
    public static class JsonData
    {
        public static JsonTest Html(string target, string selector)
        {
            return new JsonTest()
            {
                Type = JsonTest.Types.Html,
                Method = "Html",
                Target = target,
                HtmlTests = new List<HtmlTest>()
                {
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.ExistsOne,
                        Selector = selector
                    }
                }
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
