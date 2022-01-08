namespace Implem.Pleasanter.Libraries.Settings
{
    public class ApiColumn
    {
        public enum KeyDisplayTypes : int
        {
            LabelText = 0,
            ColumnName = 1
        }

        public enum ValueDisplayTypes : int
        {
            DisplayValue = 0,
            Value = 1,
            Text = 2
        }

        public KeyDisplayTypes? KeyDisplayType { get; set; }
        public ValueDisplayTypes? ValueDisplayType { get; set; }
    }
}