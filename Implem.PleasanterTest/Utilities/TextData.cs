using Implem.PleasanterTest.Models;

namespace Implem.PleasanterTest.Utilities
{
    public static class TextData
    {
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

        public static TextTest CountOf(string value, int estimate)
        {
            return new TextTest()
            {
                Type = TextTest.Types.CountOf,
                Value = value,
                Estimate = estimate
            };
        }
    }
}
