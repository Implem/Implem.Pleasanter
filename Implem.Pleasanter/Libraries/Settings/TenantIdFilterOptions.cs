using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class TenantIdFilterOptions
    {
        public static void SetChoiceHash_TenantIdFilterOptions_SysLogs(
            this Column column,
            Context context,
            string searchText,
            List<string> selectedValues = null,
            int offset = 0)
        {
            column.ChoiceHash = new Dictionary<string, Choice>();
            if (Permissions.ApplySysLogsTenantFilter(context: context))
            {
                column.TotalCount = 0;
                return;
            }
            if (offset == 0 && searchText.IsNullOrEmpty() && !column.Required)
            {
                column.ChoiceHash.Add(
                    "\t",
                    new Choice(
                        value: "\t",
                        text: Displays.NotSet(context: context)));
            }
            if (!searchText.IsNullOrEmpty() && !int.TryParse(searchText, out _))
            {
                column.TotalCount = 0;
                return;
            }
            var where = Rds.TenantsWhere();
            if (!searchText.IsNullOrEmpty())
            {
                where.TenantId(searchText.ToInt());
            }
            var dataTable = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectTenants(
                    column: Rds.TenantsColumn().TenantId().Title().TenantName(),
                    where: where,
                    orderBy: Rds.TenantsOrderBy().TenantId(),
                    offset: offset,
                    pageSize: Parameters.General.DropDownSearchPageSize));
            dataTable.AsEnumerable()
                .Select(dataRow => new
                {
                    TenantId = dataRow.Int("TenantId"),
                    DisplayName = Strings.CoalesceEmpty(
                        dataRow.String("Title"),
                        dataRow.String("TenantName"))
                })
                .ForEach(o => column.ChoiceHash.Add(
                    o.TenantId.ToString(),
                    new Choice(
                        value: o.TenantId.ToString(),
                        text: TenantIdFilterOptionText(
                            tenantId: o.TenantId,
                            displayName: o.DisplayName))));
            TenantDisplayNameHash(
                context: context,
                tenantIds: selectedValues?
                    .Where(o => !column.ChoiceHash.ContainsKey(o))
                    .Select(o => int.TryParse(o, out var tenantId) ? tenantId : 0)
                    .Where(o => o > 0)
                    .Distinct()
                    .ToList())
                        .ForEach(o => column.ChoiceHash.Add(
                            o.Key.ToString(),
                            new Choice(
                                value: o.Key.ToString(),
                                text: TenantIdFilterOptionText(
                                    tenantId: o.Key,
                                    displayName: o.Value))));
            column.TotalCount = Rds.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectCount(
                    tableName: "Tenants",
                    where: where));
        }

        public static Dictionary<string, ControlData> EditChoicesOrTenantIdFilterOptions_SysLogs(
            this Column column,
            Context context,
            bool addNotSet = false,
            bool own = false)
        {
            return column?.TableName() == "SysLogs" && column?.ColumnName == "TenantId"
                ? column?.ChoiceHash?.ToDictionary(
                    o => o.Key,
                    o => new ControlData(text: o.Value.Text))
                    ?? new Dictionary<string, ControlData>()
                : column?.EditChoices(
                    context: context,
                    addNotSet: addNotSet,
                    own: own);
        }

        public static Dictionary<int, string> TenantDisplayNameHash(
            Context context, List<int> tenantIds)
        {
            var result = new Dictionary<int, string>();
            if (Permissions.ApplySysLogsTenantFilter(context: context)
                || tenantIds?.Any() != true)
            {
                return result;
            }
            Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectTenants(
                    column: Rds.TenantsColumn().TenantId().Title().TenantName(),
                    where: Rds.TenantsWhere().TenantId_In(tenantIds)))
                        .AsEnumerable()
                        .ForEach(dataRow => result.Add(
                            dataRow.Int("TenantId"),
                            Strings.CoalesceEmpty(
                                dataRow.String("Title"),
                                dataRow.String("TenantName"))));
            return result;
        }

        public static bool IsMultipleSelectionsDropDownSearch(
            this Column column, bool filter)
        {
            return column?.MultipleSelections == true || filter;
        }

        public static string TenantIdFilterOptionText(int tenantId, string displayName)
        {
            return displayName.IsNullOrEmpty()
                ? tenantId.ToString()
                : $"[{tenantId}] {displayName}";
        }
    }
}
