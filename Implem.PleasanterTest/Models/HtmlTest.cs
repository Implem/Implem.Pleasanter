namespace Implem.PleasanterTest.Models
{
    public class HtmlTest : BaseTest
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
            Text,
            SelectedOption,
            HasInformationMessage
        }

        public Types Type { get; set; }
        public string Selector { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public TextTest TextTest { get; set; }
    }
}
