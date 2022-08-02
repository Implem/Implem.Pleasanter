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
