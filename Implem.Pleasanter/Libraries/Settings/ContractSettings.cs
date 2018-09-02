using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using System;
using System.Collections.Generic;
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
        public bool? Api;
        public DateTime? Deadline;
        public Dictionary<string, bool> Extensions;

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

        public bool OverDeadline()
        {
            return Deadline?.InRange() == true
                &&  Deadline.ToDateTime() < DateTime.Now.ToLocal();
        }

        public bool UsersLimit(Context context, int number = 1)
        {
            return Users > 0
                && Rds.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectUsers(
                    column: Rds.UsersColumn().UsersCount(),
                        where: Rds.UsersWhere().TenantId(context.TenantId))) + number > Users;
        }

        public bool SitesLimit(Context context, int number = 1)
        {
            return Sites > 0
                && Rds.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectSites(
                        column: Rds.SitesColumn().SitesCount(),
                        where: Rds.SitesWhere().TenantId(context.TenantId))) + number > Sites;
        }

        public bool ItemsLimit(Context context, long siteId, int number = 1)
        {
            return Sites > 0
                && Rds.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectItems(
                        column: Rds.ItemsColumn().ItemsCount(),
                        where: Rds.ItemsWhere().SiteId(siteId))) + number > Items;
        }

        public bool Attachments()
        {
            return Parameters.BinaryStorage.Attachments && StorageSize != 0;
        }

        public bool Images()
        {
            return Parameters.BinaryStorage.Images && Attachments();
        }
    }
}