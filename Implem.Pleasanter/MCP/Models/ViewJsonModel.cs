using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.MCP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.MCP.Models
{
    [Serializable]
    public class ViewJsonModel
    {
        public bool ApiGetMailAddresses { get; private set; }

        public List<ColumnFilter> ColumnFilters { get; set; }

        public List<ColumnSorter> ColumnSorters { get; set; }

        public List<string> GridColumns { get; set; }

        public PresetFilter PresetFilter { get; set; }

        public string Search { get; set; }

        public long SiteId { get; }

        public bool HasColumnFilters =>
            ColumnFilters.Any(f =>
                f.Values?.Count > 0);

        public bool HasColumnSorters =>
            ColumnSorters.Count > 0;

        public bool HasNegatives =>
            ColumnFilters.Any(f =>
                f.Negative);

        public bool HasGridColumns =>
            GridColumns.Count > 0;

        public bool HasSearchTypes =>
            ColumnFilters.Any(f =>
                f.Values?.Count > 0 &&
                f.SearchType.HasValue);

        public bool HasSearch =>
            !string.IsNullOrEmpty(Search);

        public ViewJsonModel(long siteId)
        {
            SiteId = siteId;
            PresetFilter = new PresetFilter();
            ColumnFilters = new List<ColumnFilter>();
            ColumnSorters = new List<ColumnSorter>();
            GridColumns = new List<string>();
        }

        private static Column.SearchTypes? ResolveSearchType(
            Dictionary<string, string> columnFilterSearchTypes,
            string columnName)
        {
            if (columnFilterSearchTypes == null ||
                !columnFilterSearchTypes.TryGetValue(
                    key: columnName,
                    value: out var searchType) ||
                string.IsNullOrEmpty(searchType))
            {
                return null;
            }

            var normalized = searchType?
                .ToLowerInvariant()
                .Replace("multiple", string.Empty);

            return normalized switch
            {
                "partialmatch" => Column.SearchTypes.PartialMatchMultiple,
                "exactmatch" => Column.SearchTypes.ExactMatchMultiple,
                "forwardmatch" => Column.SearchTypes.ForwardMatchMultiple,
                _ => null
            };
        }

        public ViewJsonModel SetApiGetMailAddresses(bool apiGetMailAddresses)
        {
            ApiGetMailAddresses = apiGetMailAddresses;
            return this;
        }

        public ViewJsonModel SetPresetFilter(
            bool incomplete = false,
            bool own = false,
            bool delay = false,
            bool overdue = false,
            bool nearCompletionTime = false)
        {
            PresetFilter.Incomplete = incomplete;
            PresetFilter.Own = own;
            PresetFilter.Delay = delay;
            PresetFilter.Overdue = overdue;
            PresetFilter.NearCompletionTime = nearCompletionTime;
            return this;
        }

        public ViewJsonModel SetColumnFilters(
            Dictionary<string, List<string>> columnFilterHash,
            Dictionary<string, string> columnFilterSearchTypes = null,
            List<string> columnFilterNegatives = null)
        {
            columnFilterHash ??= new Dictionary<string, List<string>>();
            var negatives = columnFilterNegatives ?? new List<string>();

            var allKeys = columnFilterHash.Keys
                .Concat(negatives)
                .Where(key => !string.IsNullOrEmpty(value: key))
                .Distinct();

            var filters = allKeys
                .Select(key =>
                {
                    columnFilterHash.TryGetValue(
                        key: key,
                        out var values);
                    var hasValues = values?.Count > 0;
                    var isNegative = negatives.Contains(item: key);

                    return new ColumnFilter
                    {
                        Column = key,
                        Values = hasValues
                            ? values
                            : null,
                        SearchType = hasValues
                            ? ResolveSearchType(
                                columnFilterSearchTypes: columnFilterSearchTypes,
                                columnName: key)
                            : null,
                        Negative = isNegative
                    };
                })
                .Where(f => f.Values?.Count > 0 ||
                    f.Negative);

            ColumnFilters.AddRange(filters);
            return this;
        }

        public ViewJsonModel SetColumnSorters(Dictionary<string, string> columnSorterHash)
        {
            if (columnSorterHash == null ||
                columnSorterHash.Count == 0)
            {
                return this;
            }

            var sorters = columnSorterHash
                .Where(kvp => !string.IsNullOrEmpty(kvp.Key))
                .Select(kvp => new ColumnSorter
                {
                    Column = kvp.Key,
                    Order = kvp.Value?.ToLower() ?? nameof(SqlOrderBy.Types.asc)
                });

            ColumnSorters.AddRange(collection: sorters);
            return this;
        }

        public ViewJsonModel SetGridColumns(List<string> gridColumns)
        {
            if (gridColumns == null ||
                gridColumns.Count == 0)
            {
                return this;
            }

            GridColumns.AddRange(collection: gridColumns);
            return this;
        }

        public ViewJsonModel SetSearch(string search)
        {
            Search = search;
            return this;
        }
    }

    [Serializable]
    public class PresetFilter
    {
        public bool Delay { get; set; }

        public bool Incomplete { get; set; }

        public bool Own { get; set; }

        public bool NearCompletionTime { get; set; }

        public bool Overdue { get; set; }

        public IReadOnlyDictionary<string, bool> ToDictionary()
        {
            return new Dictionary<string, bool>
            {
                ["Incomplete"] = Incomplete,
                ["Own"] = Own,
                ["Delay"] = Delay,
                ["Overdue"] = Overdue,
                ["NearCompletionTime"] = NearCompletionTime,
            };
        }
    }

    [Serializable]
    public class ColumnFilter
    {
        public string Column { get; set; }

        public bool Negative { get; set; }

        public List<string> Values { get; set; }

        public Column.SearchTypes? SearchType { get; set; }

        public ColumnFilter()
        {
            Values = new List<string>();
        }
    }

    [Serializable]
    public class ColumnSorter
    {
        public string Column { get; set; }

        public string Order { get; set; }
    }
}