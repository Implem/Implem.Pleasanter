using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Implem.PleasanterTest.Models
{
    public class TextTest : BaseTest
    {
        public enum Types
        {
            Equals,
            ListEquals,
            Contains,
            CountOf,
            CheckOrder
        }

        public Types Type { get; set; }
        public object Value { get; set; }
        public int Estimate { get; set; }
        public string[] WordArray { get; set; }

        public bool ListEquals(string text)
        {
            var value1 = Value.ToString().Deserialize<List<string>>()
                .OrderBy(o => o)
                .ToJson();
            var value2 = text.Deserialize<List<string>>()
                .OrderBy(o => o)
                .ToJson();
            return value1 == value2;
        }
    }
}
