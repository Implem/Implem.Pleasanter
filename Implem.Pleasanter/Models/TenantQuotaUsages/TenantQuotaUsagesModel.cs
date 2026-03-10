using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
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
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.ServerScripts;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class TenantQuotaUsagesModel : BaseModel
    {
        public int TenantId = 0;
        public string QuotaKey = string.Empty;
        public int Usage = 0;
        public DateTime ResetTime = 0.ToDateTime();
        public int SavedTenantId = 0;
        public string SavedQuotaKey = string.Empty;
        public int SavedUsage = 0;
        public DateTime SavedResetTime = 0.ToDateTime();

        public bool TenantId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != TenantId;
            }
            return TenantId != SavedTenantId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != TenantId);
        }

        public bool QuotaKey_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != QuotaKey;
            }
            return QuotaKey != SavedQuotaKey && QuotaKey != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != QuotaKey);
        }

        public bool Usage_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != Usage;
            }
            return Usage != SavedUsage
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != Usage);
        }

        public bool ResetTime_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != ResetTime;
            }
            return ResetTime != SavedResetTime
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != ResetTime.Date);
        }

        public TenantQuotaUsagesModel(
            Context context,
            DataRow dataRow,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            if (dataRow != null)
            {
                Set(
                    context: context,
                    dataRow: dataRow,
                    tableAlias: tableAlias);
            }
            OnConstructed(context: context);
        }

        private void OnConstructing(Context context)
        {
        }

        private void OnConstructed(Context context)
        {
        }

        public void ClearSessions(Context context)
        {
        }

        public TenantQuotaUsagesModel Get(
            Context context,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            where = where ?? Rds.TenantQuotaUsagesWhereDefault(
                context: context,
                tenantQuotaUsagesModel: this);
            column = (column ?? Rds.TenantQuotaUsagesDefaultColumns());
            join = join ?? Rds.TenantQuotaUsagesJoinDefault();
            Set(context, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectTenantQuotaUsages(
                    tableType: tableType,
                    column: column,
                    join: join,
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public void SetByModel(TenantQuotaUsagesModel tenantQuotaUsagesModel)
        {
            TenantId = tenantQuotaUsagesModel.TenantId;
            QuotaKey = tenantQuotaUsagesModel.QuotaKey;
            Usage = tenantQuotaUsagesModel.Usage;
            ResetTime = tenantQuotaUsagesModel.ResetTime;
            Comments = tenantQuotaUsagesModel.Comments;
            Creator = tenantQuotaUsagesModel.Creator;
            Updator = tenantQuotaUsagesModel.Updator;
            CreatedTime = tenantQuotaUsagesModel.CreatedTime;
            UpdatedTime = tenantQuotaUsagesModel.UpdatedTime;
            VerUp = tenantQuotaUsagesModel.VerUp;
            Comments = tenantQuotaUsagesModel.Comments;
            ClassHash = tenantQuotaUsagesModel.ClassHash;
            NumHash = tenantQuotaUsagesModel.NumHash;
            DateHash = tenantQuotaUsagesModel.DateHash;
            DescriptionHash = tenantQuotaUsagesModel.DescriptionHash;
            CheckHash = tenantQuotaUsagesModel.CheckHash;
            AttachmentsHash = tenantQuotaUsagesModel.AttachmentsHash;
        }

        private void SetBySession(Context context)
        {
        }

        private void Set(Context context, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(context, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(Context context, DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach (DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "TenantId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                TenantId = dataRow[column.ColumnName].ToInt();
                                SavedTenantId = TenantId;
                            }
                            break;
                        case "QuotaKey":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                QuotaKey = dataRow[column.ColumnName].ToString();
                                SavedQuotaKey = QuotaKey;
                            }
                            break;
                        case "Usage":
                            Usage = dataRow[column.ColumnName].ToInt();
                            SavedUsage = Usage;
                            break;
                        case "ResetTime":
                            ResetTime = dataRow[column.ColumnName].ToDateTime();
                            SavedResetTime = ResetTime;
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(context, dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(context, dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                            SavedUpdatedTime = UpdatedTime.Value;
                            break;
                        case "IsHistory":
                            VerType = dataRow.Bool(column.ColumnName)
                                ? Versions.VerTypes.History
                                : Versions.VerTypes.Latest; break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                            {
                                case "Class":
                                    SetClass(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SetSavedClass(
                                        columnName: column.Name,
                                        value: GetClass(columnName: column.Name));
                                    break;
                                case "Num":
                                    SetNum(
                                        columnName: column.Name,
                                        value: new Num(
                                            dataRow: dataRow,
                                            name: column.ColumnName));
                                    SetSavedNum(
                                        columnName: column.Name,
                                        value: GetNum(columnName: column.Name).Value);
                                    break;
                                case "Date":
                                    SetDate(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDateTime());
                                    SetSavedDate(
                                        columnName: column.Name,
                                        value: GetDate(columnName: column.Name));
                                    break;
                                case "Description":
                                    SetDescription(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SetSavedDescription(
                                        columnName: column.Name,
                                        value: GetDescription(columnName: column.Name));
                                    break;
                                case "Check":
                                    SetCheck(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToBool());
                                    SetSavedCheck(
                                        columnName: column.Name,
                                        value: GetCheck(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    SetAttachments(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString()
                                            .Deserialize<Attachments>() ?? new Attachments());
                                    SetSavedAttachments(
                                        columnName: column.Name,
                                        value: GetAttachments(columnName: column.Name).ToJson());
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return Updated()
                || TenantId_Updated(context: context)
                || QuotaKey_Updated(context: context)
                || Usage_Updated(context: context)
                || ResetTime_Updated(context: context)
                || Ver_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        private bool UpdatedWithColumn(Context context, SiteSettings ss, bool paramDefault = false)
        {
            return ClassHash.Any(o => Class_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key),
                    paramDefault: paramDefault))
                || NumHash.Any(o => Num_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key),
                    paramDefault: paramDefault))
                || DateHash.Any(o => Date_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key),
                    paramDefault: paramDefault))
                || DescriptionHash.Any(o => Description_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key),
                    paramDefault: paramDefault))
                || CheckHash.Any(o => Check_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key),
                    paramDefault: paramDefault))
                || AttachmentsHash.Any(o => Attachments_Updated(
                    columnName: o.Key,
                    context: context,
                    column: ss.GetColumn(context: context, o.Key)));
        }

        public bool Updated(Context context, SiteSettings ss, bool paramDefault = false)
        {
            return UpdatedWithColumn(context: context, ss: ss, paramDefault: paramDefault)
                || TenantId_Updated(context: context)
                || QuotaKey_Updated(context: context)
                || Usage_Updated(context: context)
                || ResetTime_Updated(context: context)
                || Ver_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool WithinQuotaKeyLimit(Context context)
        {
            var quotaDetail = context.ContractSettings.Quotas[QuotaKey];
            var limit = quotaDetail.Limit;
            if (limit <= 0) return true;
            var resetInterval = quotaDetail.ResetInterval;
            var resetUnit = quotaDetail.ResetUnit;
            var today = DateTime.Now.ToDateTime().ToLocal(context: context);
            var nextResetTime = GetNextResetTime(resetInterval, resetUnit);
            if (nextResetTime <= today)
            {
                ResetTime = today;
                Usage = 0;
            }
            Usage++;
            if (Usage > limit)
            {
                return false;
            }
            UpsertApiCount(context: context);
            return true;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public DateTime GetNextResetTime(
            int resetInterval,
            string resetUnit)
        {
            if (ResetTime == default) return ResetTime;
            if (resetInterval <= 0)
            {
                return ResetTime;
            }
            return (resetUnit ?? string.Empty).ToLowerInvariant() switch
            {
                "hour" => ResetTime.AddHours(resetInterval),
                "month" => new DateTime(ResetTime.Year, ResetTime.Month, 1).AddMonths(resetInterval),
                "day" => ResetTime.Date.AddDays(resetInterval),
                _ => ResetTime.Date.AddDays(resetInterval)
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpsertApiCount(Context context)
        {
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateOrInsertTenantQuotaUsages(
                    where: Rds.TenantQuotaUsagesWhere()
                        .TenantId(TenantId)
                        .QuotaKey(QuotaKey),
                    param: Rds.TenantQuotaUsagesParam()
                        .TenantId(TenantId)
                        .QuotaKey(QuotaKey)
                        .ResetTime(ResetTime)
                        .Usage(Usage)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public TimeSpan RemainingUntilReset(Context context)
        {
            var quotaDetail = context.ContractSettings.Quotas[QuotaKey];
            var resetInterval = quotaDetail.ResetInterval;
            var resetUnit = quotaDetail.ResetUnit;
            var today = DateTime.Now.ToDateTime().ToLocal(context: context);
            var nextResetTime = GetNextResetTime(resetInterval, resetUnit);
            return (nextResetTime <= today)
                ? TimeSpan.Zero
                : nextResetTime - today;
        }
    }

    /// <summary>
    /// Fixed:
    /// </summary>
    public static class QuotaKeys
    {
        public const string McpRequests = "MCP_REQUESTS";
    }
}
