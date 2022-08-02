namespace Implem.PleasanterTest.Models
{
    public class TextTest
    {
        public enum Types
        {
            Equals,
            Contains
        }

        public Types Type { get; set; }
        public object Value { get; set; }
    }
}
