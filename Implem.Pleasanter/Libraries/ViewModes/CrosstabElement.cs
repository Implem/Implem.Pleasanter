namespace Implem.Pleasanter.Libraries.ViewModes
{
    public class CrosstabElement
    {
        public string GroupByX;
        public string GroupByY;
        public decimal Value;

        public CrosstabElement(string groupByX, string groupByY, decimal value)
        {
            GroupByX = groupByX;
            GroupByY = groupByY;
            Value = value;
        }
    }
}