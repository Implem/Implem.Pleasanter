namespace Implem.Pleasanter.Libraries.Models
{
    public static class CsvUtilities
    {
        public static string EncloseDoubleQuotes(
            string value,
            bool? encloseDoubleQuotes)
        {
            return encloseDoubleQuotes != false
                ? "\"" + value?.Replace("\"", "\"\"") + "\""
                : value;
        }
    }
}
