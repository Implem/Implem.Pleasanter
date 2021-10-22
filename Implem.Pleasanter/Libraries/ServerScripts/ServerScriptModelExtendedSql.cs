using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelExtendedSql
    {
        private readonly Context Context;
        private readonly bool OnTesting;

        public ServerScriptModelExtendedSql(Context context, bool onTesting)
        {
            Context = context;
            OnTesting = onTesting;
        }

        public dynamic ExecuteDataSet(string name, object _params = null)
        {
            dynamic dataSet = ExtensionUtilities.ExecuteDataSetAsDynamic(
                context: Context,
                name: name,
                _params: _params?.ToString().Deserialize<Dictionary<string, object>>());
            return dataSet;
        }

        public dynamic ExecuteTable(string name, object _params = null)
        {
            dynamic dataSet = ExecuteDataSet(
                name: name,
                _params: _params);
            if (((IDictionary<string, object>)dataSet).ContainsKey("Table")) {
                return dataSet?.Table;
            }
            return null;
        }

        public dynamic ExecuteRow(string name, object _params = null)
        {
            dynamic dataTable = ExecuteTable(
                name: name,
                _params: _params);
            return ((IList<object>)dataTable)?.FirstOrDefault();
        }

        public object ExecuteScalar(string name, object _params = null)
        {
            dynamic dataRow = ExecuteRow(
                name: name,
                _params: _params);
            return ((IDictionary<string, object>)dataRow)?.FirstOrDefault().Value;
        }

        public void ExecuteNonQuery(string name, object _params = null)
        {
            _ = ExecuteTable(
                name: name,
                _params: _params);
        }
    }
}