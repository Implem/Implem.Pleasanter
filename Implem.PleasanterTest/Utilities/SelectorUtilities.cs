namespace Implem.PleasanterTest.Utilities
{
    internal class SelectorUtilities
    {
        public static string GridRow(long id)
        {
            var selector = $"#Grid tr[data-id=\"{id}\"]";
            return selector;
        }
    }
}
