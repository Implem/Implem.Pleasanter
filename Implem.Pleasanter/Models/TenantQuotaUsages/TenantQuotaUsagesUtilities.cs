using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    public static class TenantQuotaUsagesUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool TryWithinQuotaKeyLimit(
            Context context,
            string quotaKey,
            out Error.Types errorType,
            out string[] errorData)
        {
            errorType = Error.Types.None;
            errorData = null;
            if (context?.ContractSettings?.Quotas?.ContainsKey(quotaKey) != true)
            {
                return true;
            }
            var quotaDetail = context.ContractSettings.Quotas[quotaKey];
            var tenantQuotaUsagesModel = new TenantQuotaUsagesModel(
                context: context,
                dataRow: null)
            {
                TenantId = context.TenantId,
                QuotaKey = quotaKey
            };
            tenantQuotaUsagesModel.Get(
                context: context,
                where: Rds.TenantQuotaUsagesWhere()
                    .TenantId(context.TenantId)
                    .QuotaKey(quotaKey));
            if (!tenantQuotaUsagesModel.WithinQuotaKeyLimit(context: context))
            {
                errorType = Error.Types.OverTenantQuota;
                errorData = new string[]
                {
                    quotaKey,
                    quotaDetail?.Limit.ToString() ?? "0",
                    FormatResetInterval(quotaDetail)
                };
                return false;
            }
            return true;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string FormatResetInterval(QuotaDetails quotaDetail)
        {
            if (quotaDetail == null) return string.Empty;
            var interval = quotaDetail.ResetInterval;
            return (quotaDetail.ResetUnit ?? "day").ToLowerInvariant() switch
            {
                "hour" => interval == 1 ? "Hour" : $"{interval} Hours",
                "month" => interval == 1 ? "Month" : $"{interval} Months",
                _ => interval == 1 ? "Day" : $"{interval} Days"
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder RenderQuotaFieldSet(this HtmlBuilder hb, Context context)
        {
            var quotas = context.ContractSettings.Quotas;
            if(quotas is null) return hb;
            var tenantQuotaUsagesModels = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectTenantQuotaUsages(
                    column: Rds.TenantQuotaUsagesDefaultColumns(),
                    where: Rds.TenantQuotaUsagesWhere()
                        .TenantId(context.TenantId)))
                .AsEnumerable()
                .Select(dataRow => new TenantQuotaUsagesModel(
                    context: context,
                    dataRow: dataRow));
            foreach (var (key, quotaDetail) in quotas)
            {
                var tenantQuotaUsagesModel = tenantQuotaUsagesModels.FirstOrDefault(m => m.QuotaKey == key)
                                                ?? new TenantQuotaUsagesModel(
                                                    context: context,
                                                    dataRow: null)
                                                {
                                                    TenantId = context.TenantId,
                                                    QuotaKey = key,
                                                    Usage = 0,
                                                    ResetTime = default
                                                };
                var legendText = key switch
                {
                    QuotaKeys.McpRequests => Displays.McpRequests(context),
                    _ => ""
                };
                var nextReset = tenantQuotaUsagesModel.GetNextResetTime(quotaDetail.ResetInterval, quotaDetail.ResetUnit);
                hb.FieldSet(
                    id: "",
                    css: " enclosed checkField",
                    legendText: legendText,
                    action: () => hb
                        .FieldText(
                            fieldCss: "field-normal limit",
                            labelText: Displays.Limit(context: context),
                            text: quotaDetail.LimitSummary())
                        .FieldText(
                            fieldCss: "field-normal usage",
                            labelText: Displays.Usage(context: context),
                            text: tenantQuotaUsagesModel.Usage.ToString())
                        .FieldText(
                            fieldId: "field-normal nextReset",
                            labelText: Displays.NextReset(context: context),
                            text: nextReset == default ? "-" : nextReset.ToString(Displays.YmdhmsFormat(context: context))),
                    _using: quotaDetail.Limit > 0
                );
            }
            return hb;
        }
    }
}
