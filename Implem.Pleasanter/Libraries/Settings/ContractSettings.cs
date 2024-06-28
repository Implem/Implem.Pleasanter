using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable()]
    public class ContractSettings
    {
        public string Name;
        public string DisplayName;
        public int? Users;
        public long? Sites;
        public long? Items;
        public decimal? StorageSize;
        public bool? Import;
        public bool? Export;
        public bool? Notice;
        public bool? Remind;
        public bool? Mail;
        public bool? Style;
        public bool? Script;
        public bool? Html;
        public bool? ServerScript;
        public bool? Api;
        public DateTime? Deadline;
        public Dictionary<string, bool> Extensions;
        public List<string> AllowIpAddresses;
        public string SamlCompanyCode;
        public string SamlThumbprint;
        public string SamlLoginUrl;
        public string SamlMetadataGuid;
        public int? AllowOriginalLogin;
        public bool? AllowNewFeatures;
        public int? ApiLimitPerSite;

        public ContractSettings()
        {
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext streamingContext)
        {
        }

        public string RecordingJson()
        {
            return null;
        }

        public bool InitialValue(Context context)
        {
            return this.ToJson() == "[]";
        }

        public bool OverDeadline(Context context)
        {
            return Deadline?.InRange() == true
                && Deadline.ToDateTime() < DateTime.Now.ToLocal(context: context);
        }

        public bool UsersLimit(Context context, int number = 1)
        {
            return Users > 0
                && Repository.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectUsers(
                    column: Rds.UsersColumn().UsersCount(),
                        where: Rds.UsersWhere().TenantId(context.TenantId))) + number > Users;
        }

        public bool SitesLimit(Context context, int number = 1)
        {
            return Sites > 0
                && Repository.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectSites(
                        column: Rds.SitesColumn().SitesCount(),
                        where: Rds.SitesWhere().TenantId(context.TenantId))) + number > Sites;
        }

        public bool ItemsLimit(Context context, long siteId, int number = 1)
        {
            return Items > 0
                && Repository.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectItems(
                        column: Rds.ItemsColumn().ItemsCount(),
                        where: Rds.ItemsWhere()
                            .SiteId(siteId)
                            .ReferenceType("Sites", _operator: "<>"))) + number > Items;
        }

        public bool OverTenantStorageSize(
            decimal totalFileSize, decimal newTotalFileSize, decimal? limit)
        {
            if (limit != null &&
                (totalFileSize + newTotalFileSize) > limit * 1024 * 1024 * 1024) return true;
            return false;
        }

        public bool Attachments()
        {
            return Parameters.BinaryStorage.Attachments && StorageSize != 0;
        }

        public bool Images()
        {
            return Parameters.BinaryStorage.Images && Attachments();
        }

        public bool AllowedIpAddress(Context context, string ipAddress)
        {
            return IpAddresses.AllowedIpAddress(
                context: context,
                allowIpAddresses: AllowIpAddresses,
                ipRestrictionExcludeMembers: Parameters.Security.IpRestrictionExcludeMembers,
                ipAddress: ipAddress);
        }

        public bool NewFeatures()
        {
            return AllowNewFeatures == true || !Parameters.Service.RestrictNewFeatures;
        }

        public int ApiLimit()
        {
            return (ApiLimitPerSite != null)
                ? (int)ApiLimitPerSite
                : Parameters.Api.LimitPerSite;
        }
    }
}