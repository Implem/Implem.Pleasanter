using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

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
        public readonly Dictionary<string, bool> FilterNegatives = new Dictionary<string, bool>();
        public readonly ExpandoObject SearchTypes = new ExpandoObject();
        public readonly ExpandoObject Sorters = new ExpandoObject();
        public bool FiltersCleared { private set; get; }
        private List<string> ChangedFilters = new List<string>();
        private bool Initialized { get; set; }

        public ServerScriptModelView(int id = 0)
        {
            Id = id;
            ((INotifyPropertyChanged)Filters).PropertyChanged += FiltersChanged;
        }

        public void AddColumnPlaceholder(string key, string value)
        {
            if (ColumnPlaceholders == null)
            {
                ColumnPlaceholders = new Dictionary<string, string>();
            }
            ColumnPlaceholders.AddOrUpdate(key, value);
        }

        public void ClearFilters()
        {
            ((IDictionary<string, object>)Filters).Clear();
            FiltersCleared = true;
        }

        public void FilterNegative(string name, bool negative = true)
        {
            FilterNegatives.AddOrUpdate(name, negative);
        }

        internal void SetInitialized()
        {
            Initialized = true;
        }

        private void FiltersChanged(object sender, PropertyChangedEventArgs e)
        {
            FiltersChanged(
                filters: Filters,
                name: e.PropertyName);
        }

        private void FiltersChanged(
            IDictionary<string, object> filters,
            string name)
        {
            if (Initialized && !ChangedFilters.Contains(name))
            {
                ChangedFilters.Add(name);
                if (name.StartsWith("or_") || name.StartsWith("and_"))
                {
                    var childFilters = filters[name].ToString()
                        .Deserialize<Dictionary<string, object>>()
                        ?.ToDictionary(
                            o => name + "\\" + o.Key,
                            o => o.Value);
                    if (childFilters != null)
                    {
                        foreach (var childName in childFilters.Keys)
                        {
                            FiltersChanged(
                                filters: childFilters,
                                name: childName);
                        }
                    }
                }
            }
        }

        internal void ClearColumnFilterNegatives(View view)
        {
            view.ColumnFilterNegatives?.RemoveAll(o => ChangedFilters.Contains(o));
        }
    }
}