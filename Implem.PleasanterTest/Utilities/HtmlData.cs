using Implem.Libraries.Utilities;
using Implem.PleasanterTest.Models;

namespace Implem.PleasanterTest.Utilities
{
    public static class HtmlData
    {
        public static HtmlTest Exists(string selector)
        {
            return new HtmlTest()
            {
                Type = HtmlTest.Types.Exists,
                Selector = selector
            };
        }
        public static HtmlTest ExistsOne(string selector)
        {
            return new HtmlTest()
            {
                Type = HtmlTest.Types.ExistsOne,
                Selector = selector
            };
        }

        public static HtmlTest NotExists(string selector)
        {
            return new HtmlTest()
            {
                Type = HtmlTest.Types.NotExists,
                Selector = selector
            };
        }

        public static HtmlTest TextContent(string selector, string value)
        {
            return new HtmlTest()
            {
                Type = HtmlTest.Types.TextContent,
                Selector = selector,
                Value = value
            };
        }

        public static HtmlTest Disabled(string selector)
        {
            return new HtmlTest()
            {
                Type = HtmlTest.Types.Disabled,
                Selector = selector,
            };
        }

        public static HtmlTest InnerHtml(string selector, string value)
        {
            return new HtmlTest()
            {
                Type = HtmlTest.Types.InnerHtml,
                Selector = selector,
                Value = value
            };
        }

        public static HtmlTest TextContains(string selector, string value)
        {
            return new HtmlTest()
            {
                Type = HtmlTest.Types.Text,
                Selector = selector,
                TextTest = new TextTest()
                {
                    Type = TextTest.Types.Contains,
                    Value = value
                }
            };
        }

        public static HtmlTest TextCountOf(
            string selector,
            string value,
            int estimate)
        {
            return new HtmlTest()
            {
                Type = HtmlTest.Types.Text,
                Selector = selector,
                TextTest = new TextTest()
                {
                    Type = TextTest.Types.CountOf,
                    Value = value,
                    Estimate = estimate
                }
            };
        }

        public static HtmlTest TextCheckOrder(
            string selector,
            string[] wordArray)
        {
            return new HtmlTest()
            {
                Type = HtmlTest.Types.Text,
                Selector = selector,
                TextTest = new TextTest()
                {
                    Type = TextTest.Types.CheckOrder,
                    WordArray = wordArray
                }
            };
        }

        public static HtmlTest NotFoundMessage()
        {
            return new HtmlTest()
            {
                Type = HtmlTest.Types.NotFoundMessage,
            };
        }

        public static HtmlTest HasNotPermissionMessage()
        {
            return new HtmlTest()
            {
                Type = HtmlTest.Types.HasNotPermissionMessage,
            };
        }

        public static HtmlTest HasInformationMessage(string message)
        {
            return new HtmlTest()
            {
                Type = HtmlTest.Types.HasInformationMessage,
                Value = message
            };
        }

        public static HtmlTest SelectedOption(string selector, string value)
        {
            return new HtmlTest()
            {
                Type = HtmlTest.Types.SelectedOption,
                Selector = selector,
                Value = value
            };
        }

        public static HtmlTest Attribute(string selector, string name, string value)
        {
            return new HtmlTest()
            {
                Name = name,
                Type = HtmlTest.Types.Attribute,
                Selector = selector,
                Value = value
            };
        }

    }
}
