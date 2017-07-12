using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Models
{
    public static class Imports
    {
        public static string ColumnValidate(
            SiteSettings ss, IEnumerable<string> headers, params string[] columnNames)
        {
            foreach (var name in columnNames)
            {
                if (!headers.Contains(name))
                {
                    return Messages.ResponseNotRequiredColumn(
                        ss.GetColumn(name).LabelText).ToJson();
                }
            }
            return null;
        }

        public static string Validate(Dictionary<int, string> hash, Column column)
        {
            foreach (var data in hash.Where(o => HasError(o.Value, column)))
            {
                return Messages.ResponseInvalidCsvData(
                    (data.Key + 2).ToString(), column.LabelText).ToJson();
            }
            return null;
        }

        private static bool HasError(string data, Column column)
        {
            switch (column.TypeName)
            {
                case "datetime": return !Times.InRange(data.ToDateTime());
                default: return data == null;
            }
        }
    }
}