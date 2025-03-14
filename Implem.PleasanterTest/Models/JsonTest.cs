using System.Collections.Generic;
namespace Implem.PleasanterTest.Models
{
    public class JsonTest : BaseTest
    {
        public enum Types
        {
            Value,
            Exists,
            ExistsOne,
            Html,
            Message,
            Text
        }

        public Types Type { get; set; }
        public string Method { get; set; }
        public string Target { get; set; }
        public object Value { get; set; }
        public string Options { get; set; }
        public List<HtmlTest> HtmlTests { get; set; }
        public TextTest TextTest { get; set; }
    }
}
