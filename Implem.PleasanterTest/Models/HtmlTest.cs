using System.Collections.Generic;

namespace Implem.PleasanterTest.Models
{
    public class HtmlTest
    {
        public enum Types
        {
            TextContent,
            InnerHtml,
            Exists,
            ExistsOne,
            NotExists,
            Disabled,
            HasClass,
            Attribute,
            NotFoundMessage,
            HasNotPermissionMessage,
            Text
        }

        public Types Type { get; set; }
        public string Selector { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public List<TextTest> TextTests { get; set; }
    }
}
