using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Dynamic;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelView
    {
        public readonly int Id;
        public List<string> AlwaysGetColumns = new List<string>();
        public string OnSelectingWhere;
        public string OnSelectingOrderBy;
        public Dictionary<string, string> ColumnPlaceholders;
        public readonly ExpandoObject Filters = new ExpandoObject();
        public readonly ExpandoObject SearchTypes = new ExpandoObject();
        public readonly ExpandoObject Sorters = new ExpandoObject();

        public ServerScriptModelView(int id = 0)
        {
            Id = id;
        }

        public void AddColumnPlaceholder(string key, string value)
        {
            if (ColumnPlaceholders == null)
            {
                ColumnPlaceholders = new Dictionary<string, string>();
            }
            ColumnPlaceholders.AddOrUpdate(key, value);
        }
    }
}