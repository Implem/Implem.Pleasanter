namespace Implem.PleasanterTest.Models
{
    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public KeyValue(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
