namespace Implem.Pleasanter.Libraries.ViewModes
{
    public class AnalyPartElement
    {
        public string GroupValue;
        public string GroupTitle;
        public decimal Value;

        public AnalyPartElement(
            string groupValue,
            string groupTitle,
            decimal value)
        {
            GroupValue = groupValue;
            GroupTitle = groupTitle;
            Value = value;
        }
    }
}