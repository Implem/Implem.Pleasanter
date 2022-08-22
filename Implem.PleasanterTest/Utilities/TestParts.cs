using Implem.PleasanterTest.Models;
using System.Collections.Generic;

namespace Implem.PleasanterTest.Utilities
{
    internal static class TestParts
    {
        public static object[] ExistsOne(string selector)
        {
            return new object[]
            {
                new List<BaseTest>()
                {
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.ExistsOne,
                        Selector = selector
                    }
                }
            };
        }

        public static object[] NotExists(string selector)
        {
            return new object[]
            {
                new List<BaseTest>()
                {
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.NotExists,
                        Selector = selector
                    }
                }
            };
        }

        public static object[] Disabled(string selector)
        {
            return new object[]
            {
                new List<BaseTest>()
                {
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.Disabled,
                        Selector = selector
                    }
                }
            };
        }

        public static object[] HasClass(string selector, string value)
        {
            return new object[]
            {
                new List<BaseTest>()
                {
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.HasClass,
                        Selector = selector,
                        Value = value
                    }
                }
            };
        }

        public static object[] Attribute(string selector, string name, string value)
        {
            return new object[]
            {
                new List<BaseTest>()
                {
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.Attribute,
                        Selector = selector,
                        Name = name,
                        Value = value
                    }
                }
            };
        }

        public static object[] ValidateRequired(string selector)
        {
            return new object[]
            {
                new List<BaseTest>()
                {
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.Attribute,
                        Selector = selector,
                        Name = "data-validate-required",
                        Value = "1"
                    }
                }
            };
        }

        public static object[] ValidateAttachmentsRequired(string selector)
        {
            return new object[]
            {
                new List<BaseTest>()
                {
                    new HtmlTest()
                    {
                        Type = HtmlTest.Types.Attribute,
                        Selector = selector,
                        Name = "data-validate-attachments-required",
                        Value = "1"
                    }
                }
            };
        }
    }
}
