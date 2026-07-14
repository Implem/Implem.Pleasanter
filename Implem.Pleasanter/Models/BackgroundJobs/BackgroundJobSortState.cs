using System;
using System.Collections.Generic;
using System.Linq;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;

namespace Implem.Pleasanter.Models
{
    public class BackgroundJobSortState
    {
        private readonly Dictionary<string, string> _sorters;

        public BackgroundJobSortState(Dictionary<string, string> sorters)
        {
            _sorters = sorters ?? new Dictionary<string, string>();
        }

        public bool HasSorters => _sorters.Count > 0;

        public string NextOrderType(string columnName)
        {
            return _sorters.TryGetValue(columnName, out var current)
                ? current == "asc" ? "desc" : string.Empty
                : "asc";
        }

        public string CurrentOrderType(string columnName)
        {
            return _sorters.TryGetValue(columnName, out var current) ? current : string.Empty;
        }

        public IEnumerable<KeyValuePair<string, string>> GetSorters()
        {
            return _sorters;
        }

        public static BackgroundJobSortState FromContext(Context context)
        {
            var sorters = new Dictionary<string, string>();
            var prefix = "ViewSorters__";
            foreach (var key in context.Forms.Keys.Where(k => k.StartsWith(prefix)))
            {
                var col = key.Substring(prefix.Length);
                var val = context.Forms.Data(key);
                if (val.IsNullOrEmpty() == false)
                {
                    sorters[col] = val;
                }
            }
            return new BackgroundJobSortState(sorters);
        }
    }
}
