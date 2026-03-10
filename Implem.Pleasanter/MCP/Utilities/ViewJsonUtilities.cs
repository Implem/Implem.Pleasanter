using Implem.Libraries.DataSources.SqlServer;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.MCP.Models;
using Implem.Pleasanter.MCP.Translator;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using static Implem.Pleasanter.MCP.Utilities.CommonUtilities;

namespace Implem.Pleasanter.MCP.Utilities
{
    public static class ViewJsonUtilities
    {
        public static string CreateViewJson(
            long siteId,
            bool incomplete = false,
            bool own = false,
            bool delay = false,
            bool overdue = false,
            bool nearCompletionTime = false,
            string search = "",
            Dictionary<string, List<string>> columnFilterHash = null,
            Dictionary<string, string> columnFilterSearchTypes = null,
            List<string> columnFilterNegatives = null,
            Dictionary<string, string> columnSorterHash = null,
            List<string> gridColumns = null,
            bool apiGetMailAddresses = false)
        {
            var viewModel = new ViewJsonModel(siteId)
                .SetPresetFilter(
                    incomplete: incomplete,
                    own: own,
                    delay: delay,
                    overdue: overdue,
                    nearCompletionTime: nearCompletionTime)
                .SetSearch(search)
                .SetColumnFilters(
                    columnFilterHash: columnFilterHash,
                    columnFilterSearchTypes: columnFilterSearchTypes,
                    columnFilterNegatives: columnFilterNegatives)
                .SetColumnSorters(columnSorterHash)
                .SetGridColumns(gridColumns)
                .SetApiGetMailAddresses(apiGetMailAddresses);

            return BuildViewJson(viewJsonModel: viewModel);
        }

        private static Dictionary<string, string> BuildColumnFilterHash(
            List<ColumnFilter> columnFilters,
            CodeTranslator codeTranslator,
            long siteId)
        {
            var context = CreateContext(siteId: siteId);
            var ss = SiteSettingsUtilities.Get(
                context: context,
                siteId: siteId);

            var hash = new Dictionary<string, string>();

            var filtersWithValues = columnFilters
                .Where(filter => filter.Values?.Count > 0);

            foreach (var filter in filtersWithValues)
            {
                var columnName = codeTranslator.LabelToColumn(filter.Column);

                var translatedValues = filter.Values
                    .Select(v => codeTranslator.TranslateToCode(
                        columnName: columnName,
                        displayValue: v))
                    .ToList();

                var isClassFreeInput = IsClassColumnWithEmptyChoices(
                    ss: ss,
                    context: context,
                    columnName: columnName);

                var jsonValue = isClassFreeInput
                    ? translatedValues.First()
                    : JsonConvert.SerializeObject(value: translatedValues);

                hash[columnName] = jsonValue;
            }

            return hash;
        }

        private static bool IsClassColumnWithEmptyChoices(
            SiteSettings ss,
            Context context,
            string columnName)
        {
            var column = ss.GetColumn(
                context: context,
                columnName: columnName);

            var isClassColumn = column?.ColumnName.StartsWith(value: "Class") == true;
            var hasNoChoicesText = string.IsNullOrEmpty(value: column?.ChoicesText);

            return isClassColumn && hasNoChoicesText;
        }

        private static List<string> BuildColumnFilterNegatives(
            List<ColumnFilter> columnFilters,
            CodeTranslator codeTranslator)
        {
            return columnFilters
                .Where(filter => filter.Negative)
                .Select(filter => codeTranslator.LabelToColumn(filter.Column))
                .ToList();
        }

        private static Dictionary<string, Column.SearchTypes> BuildColumnFilterSearchTypes(
            List<ColumnFilter> columnFilters,
            CodeTranslator codeTranslator)
        {
            return columnFilters
                .Where(filter => filter.SearchType.HasValue)
                .ToDictionary(
                    filter => codeTranslator.LabelToColumn(filter.Column),
                    filter => filter.SearchType.Value);
        }

        private static Dictionary<string, SqlOrderBy.Types> BuildColumnSorterHash(
            List<ColumnSorter> columnSorter,
            CodeTranslator codeTranslator)
        {
            var hash = new Dictionary<string, SqlOrderBy.Types>();

            foreach (var sorter in columnSorter)
            {
                var columnName = codeTranslator.LabelToColumn(sorter.Column);

                var orderType = sorter.Order.ToLower() == nameof(SqlOrderBy.Types.desc)
                    ? SqlOrderBy.Types.desc
                    : SqlOrderBy.Types.asc;

                hash[columnName] = orderType;
            }

            return hash;
        }

        private static List<string> BuildGridColumns(
            List<string> gridColumns,
            CodeTranslator codeTranslator)
        {
            return gridColumns
                .Select(column => TranslateGridColumn(
                    column: column,
                    codeTranslator: codeTranslator))
                .ToList();
        }

        private static string BuildViewJson(ViewJsonModel viewJsonModel)
        {
            var viewObject = BuildViewObject(viewJsonModel: viewJsonModel);

            return JsonConvert.SerializeObject(
                value: viewObject,
                formatting: Formatting.None,
                settings: new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        private static object BuildViewObject(ViewJsonModel viewJsonModel)
        {
            var viewObject = new Dictionary<string, object>{};

            foreach (var (key, enabled) in viewJsonModel.PresetFilter.ToDictionary())
            {
                if (enabled)
                {
                    viewObject[key] = true;
                }
            }

            if (viewJsonModel.HasSearch)
            {
                viewObject["Search"] = viewJsonModel.Search;
            }

            var codeTranslator = CommonUtilities.Translator(siteId: viewJsonModel.SiteId);

            if (viewJsonModel.HasColumnFilters)
            {
                viewObject["ColumnFilterHash"] =
                    BuildColumnFilterHash(
                        columnFilters: viewJsonModel.ColumnFilters,
                        codeTranslator: codeTranslator,
                        siteId: viewJsonModel.SiteId);
            }

            if (viewJsonModel.HasSearchTypes)
            {
                viewObject["ColumnFilterSearchTypes"] =
                    BuildColumnFilterSearchTypes(
                        columnFilters: viewJsonModel.ColumnFilters,
                        codeTranslator: codeTranslator);
            }

            if (viewJsonModel.HasNegatives)
            {
                viewObject["ColumnFilterNegatives"] =
                    BuildColumnFilterNegatives(
                        columnFilters: viewJsonModel.ColumnFilters,
                        codeTranslator: codeTranslator);
            }

            if (viewJsonModel.HasColumnSorters)
            {
                viewObject["ColumnSorterHash"] =
                    BuildColumnSorterHash(
                        columnSorter: viewJsonModel.ColumnSorters,
                        codeTranslator: codeTranslator);
            }

            if (viewJsonModel.HasGridColumns)
            {
                viewObject["GridColumns"] =
                    BuildGridColumns(
                        gridColumns: viewJsonModel.GridColumns,
                        codeTranslator: codeTranslator);
            }

            if (viewJsonModel.ApiGetMailAddresses)
            {
                viewObject["ApiGetMailAddresses"] = true;
            }

            return viewObject;
        }

        private static string TranslateGridColumn(
            string column,
            CodeTranslator codeTranslator)
        {
            if (!column.Contains("~"))
            {
                return codeTranslator.LabelToColumn(column);
            }

            var pattern = @"(~\d+,)";
            var parts = System.Text.RegularExpressions.Regex.Split(column, pattern);

            var translatedParts = parts
                .Where(part => !string.IsNullOrEmpty(part))
                .Select(part =>
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(part, @"^~\d+,$"))
                    {
                        return part;
                    }
                    return codeTranslator.LabelToColumn(part);
                });

            return string.Join("", translatedParts);
        }
    }
}