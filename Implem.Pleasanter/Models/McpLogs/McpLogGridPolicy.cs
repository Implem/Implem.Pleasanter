using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class McpLogGridPolicy
    {
        private const int DefaultStartTimeRangeDays = 31;

        public static bool ApplyDefaultStartTimeRange(
            Context context,
            SiteSettings ss,
            View view)
        {
            if (ss.ReferenceType != "McpLogs"
                || HasStartTimeFilter(view)
                || HasPartialMatchDataFilter(context: context, ss: ss, view: view) == false)
            {
                return false;
            }
            var column = ss.GetColumn(
                context: context,
                columnName: "StartTime");
            if (column == null)
            {
                return false;
            }
            var today = DateTime.Now.ToLocal(context: context).Date;
            var days = Parameters.McpServer?.LogPartialMatchDefaultStartTimeRangeDays
                ?? DefaultStartTimeRangeDays;
            var maxDays = (today - Parameters.General.MinTime).Days + 1;
            if (days <= 0 || days > maxDays)
            {
                days = DefaultStartTimeRangeDays;
            }
            var start = today.AddDays(1 - days);
            var range = column.DisplayControl(
                    context: context,
                    value: start)
                + ",";
            if (view.ColumnFilterHash == null)
            {
                view.ColumnFilterHash = new Dictionary<string, string>();
            }
            view.ColumnFilterHash["StartTime"] = new[] { range }.ToJson();
            return true;
        }

        private static bool HasPartialMatchDataFilter(
            Context context,
            SiteSettings ss,
            View view)
        {
            return HasPartialMatchFilter(
                    context: context,
                    ss: ss,
                    view: view,
                    columnName: "ResponseData")
                || HasPartialMatchFilter(
                    context: context,
                    ss: ss,
                    view: view,
                    columnName: "RequestData");
        }

        private static bool HasPartialMatchFilter(
            Context context,
            SiteSettings ss,
            View view,
            string columnName)
        {
            var column = ss.GetColumn(
                context: context,
                columnName: columnName);
            var searchType = view.ColumnFilterSearchTypes?.ContainsKey(columnName) == true
                ? view.ColumnFilterSearchTypes[columnName]
                : column?.SearchType;
            var value = view.ColumnFilter(columnName);
            switch (searchType)
            {
                case Column.SearchTypes.PartialMatch:
                    return string.IsNullOrWhiteSpace(value) == false;
                case Column.SearchTypes.PartialMatchMultiple:
                    return value
                        .Deserialize<List<string>>()
                        ?.Any(o => o != "\t") == true;
                default:
                    return false;
            }
        }

        private static bool HasStartTimeFilter(View view)
        {
            return HasStartTimeValue(view.ColumnFilter("StartTime"))
                || view.ColumnFilterExpressions?.ContainsKey("StartTime") == true
                || view.ColumnFilterNegatives?.Contains("StartTime") == true;
        }

        private static bool HasStartTimeValue(string value)
        {
            return value
                .Deserialize<List<string>>()
                ?.Any(o => o.IsNullOrEmpty() == false) == true;
        }
    }
}