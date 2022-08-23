namespace Implem.PleasanterTest.Models
{
    public class ApiJsonTest : BaseTest
    {
        public enum Types
        {
            StatusCode
        }

        public Types Type { get; set; }
        public object Value { get; set; }
    }
}
