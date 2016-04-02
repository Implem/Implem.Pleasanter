using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
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
            Dictionary<int, SqlParamCollection> paramHash, Column column)
        {
            foreach (var data in paramHash.Where(o => o.Value.Any(p => HasError(p, column))))
            {
                return Messages.ResponseInvalidCsvData(
                    data.Key.ToString(), column.LabelText).ToJson();
            }
            return null;
        }

        private static bool HasError(SqlParam sqlParam, Column column)
        {
            if (sqlParam.Name == column.ColumnName)
            {
                switch (column.TypeName)
                {
                    case "datetime": return !Times.InRange(sqlParam.Value.ToDateTime());
                    default: return sqlParam.Value == null;
                }
            }
            else
            {
                return false;
            }
        }
    }
}