namespace Implem.PleasanterTest.Models
{
    public class HtmlTest
    {
        public enum Types
        {
            TextContent,
            InnerHtml,
            Exists,
            ExistsOne
        }

        public Types Type { get; set; }
        public string Selector { get; set; }
        public string Value { get; set; }
    }
}
