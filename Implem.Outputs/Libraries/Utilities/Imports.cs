using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Utilities
{
    public static class Imports
    {
        public static string ColumnValidate(
            SiteSettings siteSettings, IEnumerable<string> headers, params string[] columnNames)
        {
            foreach (var name in columnNames)
            {
                if (!headers.Contains(name))
                {
                    return Messages.ResponseNotRequiredColumn(
                        siteSettings.AllColumn(name).LabelText).ToJson();
                }
            }
            return null;
        }

        public static string Validate(
            Dictionary<int, SqlParamCollection> paramHash, string columnName, string labelText)
        {
            foreach (var data in paramHash.Where(o =>
                o.Value.Any(p => p.Name == columnName && p.Value == null)))
            {
                return Messages.ResponseInvalidCsvData(data.Key.ToString(), labelText).ToJson();
            }
            return null;
        }
    }
}