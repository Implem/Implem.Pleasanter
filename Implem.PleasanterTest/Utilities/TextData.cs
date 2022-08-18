using Implem.PleasanterTest.Models;
using System.Collections.Generic;
using System.Linq;

namespace Implem.PleasanterTest.Utilities
{
    public static class TextData
    {
        public static List<TextTest> Tests(params TextTest[] tests)
        {
            return tests.ToList();
        }

        public static TextTest Equals(string value)
        {
            return new TextTest()
            {
                Type = TextTest.Types.Equals,
                Value = value
            };
        }

        public static TextTest ListEquals(string value)
        {
            return new TextTest()
            {
                Type = TextTest.Types.ListEquals,
                Value = value
            };
        }

        public static TextTest Contains(string value)
        {
            return new TextTest()
            {
                Type = TextTest.Types.Contains,
                Value = value
            };
        }
    }
}
