using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class SiteSettingsUtilities
    {
        public static SiteSettings Get(
            long siteId,
            long referenceId = 0,
            bool setSiteIntegration = false,
            bool setAllChoices = false)
        {
            return Get(
                new SiteModel(siteId),
                referenceId != 0
                    ? referenceId
                    : siteId,
                setSiteIntegration: setSiteIntegration,
                setAllChoices: setAllChoices);
        }

        public static SiteSettings Get(DataRow dataRow)
        {
            return dataRow != null
                ? dataRow["SiteSettings"]
                    .ToString()
                    .Deserialize<SiteSettings>() ??
                        Get(dataRow["ReferenceType"].ToString(),
                            dataRow["SiteId"].ToLong())
                : null;
        }

        public static SiteSettings Get(this List<SiteSettings> ssList, long siteId)
        {
            return ssList.FirstOrDefault(o => o.SiteId == siteId);
        }

        public static View Get(this List<View> views, int? id)
        {
            return views?.FirstOrDefault(o => o.Id == id);
        }

        public static SiteSettings Get(string referenceType, long siteId)
        {
            switch (referenceType)
            {
                case "Sites": return SitesSiteSettings(siteId);
                case "Issues": return IssuesSiteSettings(siteId);
                case "Results": return ResultsSiteSettings(siteId);
                case "Wikis": return WikisSiteSettings(siteId);
                default: return new SiteSettings() { SiteId = siteId };
            }
        }

        public static SiteSettings Get(
            SiteModel siteModel,
            long referenceId,
            bool setSiteIntegration = false,
            bool setAllChoices = false)
        {
            switch (siteModel.ReferenceType)
            {
                case "Sites": return SitesSiteSettings(
                    siteModel: siteModel,
                    referenceId: referenceId,
                    setSiteIntegration: setSiteIntegration,
                    setAllChoices: setAllChoices);
                case "Issues": return IssuesSiteSettings(
                    siteModel: siteModel,
                    referenceId: referenceId,
                    setSiteIntegration: setSiteIntegration,
                    setAllChoices: setAllChoices);
                case "Results": return ResultsSiteSettings(
                    siteModel: siteModel,
                    referenceId: referenceId,
                    setSiteIntegration: setSiteIntegration,
                    setAllChoices: setAllChoices);
                case "Wikis": return WikisSiteSettings(
                    siteModel: siteModel,
                    referenceId: referenceId,
                    setSiteIntegration: setSiteIntegration,
                    setAllChoices: setAllChoices);
                default: return new SiteSettings() { SiteId = siteModel.SiteId };
            }
        }

        public static SiteSettings GetByReference(string reference, long referenceId)
        {
            switch (reference.ToLower())
            {
                case "tenants": return TenantsSiteSettings();
                case "demos": return DemosSiteSettings();
                case "syslogs": return SysLogsSiteSettings();
                case "statuses": return StatusesSiteSettings();
                case "reminderschedules": return ReminderSchedulesSiteSettings();
                case "healths": return HealthsSiteSettings();
                case "depts": return DeptsSiteSettings();
                case "groups": return GroupsSiteSettings();
                case "groupmembers": return GroupMembersSiteSettings();
                case "users": return UsersSiteSettings();
                case "loginkeys": return LoginKeysSiteSettings();
                case "mailaddresses": return MailAddressesSiteSettings();
                case "outgoingmails": return OutgoingMailsSiteSettings();
                case "searchindexes": return SearchIndexesSiteSettings();
                case "orders": return OrdersSiteSettings();
                case "exportsettings": return ExportSettingsSiteSettings();
                case "links": return LinksSiteSettings();
                case "binaries": return BinariesSiteSettings();
                case "items": return Get(new ItemModel(referenceId).GetSite(), referenceId);
                default: return null;
            }
        }

        public static SiteSettings TenantsSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Tenants";
            ss.Init();
            return ss;
        }

        public static SiteSettings DemosSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Demos";
            ss.Init();
            return ss;
        }

        public static SiteSettings SysLogsSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "SysLogs";
            ss.Init();
            return ss;
        }

        public static SiteSettings StatusesSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Statuses";
            ss.Init();
            return ss;
        }

        public static SiteSettings ReminderSchedulesSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "ReminderSchedules";
            ss.Init();
            return ss;
        }

        public static SiteSettings HealthsSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Healths";
            ss.Init();
            return ss;
        }

        public static SiteSettings DeptsSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Depts";
            ss.Init();
            ss.SetChoiceHash(withLink: false);
            ss.PermissionType = Permissions.Admins();
            return ss;
        }

        public static SiteSettings GroupsSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Groups";
            ss.Init();
            ss.SetChoiceHash(withLink: false);
            ss.PermissionType = Permissions.Admins();
            return ss;
        }

        public static SiteSettings GroupMembersSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "GroupMembers";
            ss.Init();
            return ss;
        }

        public static SiteSettings UsersSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Users";
            ss.Init();
            ss.SetChoiceHash(withLink: false);
            ss.PermissionType = Permissions.Admins();
            return ss;
        }

        public static SiteSettings LoginKeysSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "LoginKeys";
            ss.Init();
            return ss;
        }

        public static SiteSettings MailAddressesSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "MailAddresses";
            ss.Init();
            return ss;
        }

        public static SiteSettings OutgoingMailsSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "OutgoingMails";
            ss.Init();
            return ss;
        }

        public static SiteSettings SearchIndexesSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "SearchIndexes";
            ss.Init();
            return ss;
        }

        public static SiteSettings ItemsSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Items";
            ss.Init();
            return ss;
        }

        public static SiteSettings OrdersSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Orders";
            ss.Init();
            return ss;
        }

        public static SiteSettings ExportSettingsSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "ExportSettings";
            ss.Init();
            return ss;
        }

        public static SiteSettings LinksSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Links";
            ss.Init();
            return ss;
        }

        public static SiteSettings BinariesSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Binaries";
            ss.Init();
            return ss;
        }

        public static SiteSettings SitesSiteSettings(
            this SiteModel siteModel,
            long referenceId,
            bool setSiteIntegration = false,
            bool setAllChoices = false)
        {
            var ss = siteModel.SiteSettings ?? new SiteSettings();
            ss.SiteId = siteModel.SiteId;
            ss.ReferenceId = referenceId;
            ss.Title = siteModel.Title.Value;
            ss.ReferenceType = "Sites";
            ss.ParentId = siteModel.ParentId;
            ss.InheritPermission = siteModel.InheritPermission;
            ss.AccessStatus = siteModel.AccessStatus;
            ss.Init();
            ss.SetLinkedSiteSettings();
            ss.SetPermissions(referenceId);
            ss.SetJoinedSsHash();
            if (setSiteIntegration) ss.SetSiteIntegration();
            return ss;
        }

        public static SiteSettings SitesSiteSettings(long siteId, bool setAllChoices = false)
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Sites";
            ss.SiteId = siteId;
            ss.Init();
            return ss;
        }

        public static SiteSettings IssuesSiteSettings(
            this SiteModel siteModel,
            long referenceId,
            bool setSiteIntegration = false,
            bool setAllChoices = false)
        {
            var ss = siteModel.SiteSettings ?? new SiteSettings();
            ss.SiteId = siteModel.SiteId;
            ss.ReferenceId = referenceId;
            ss.Title = siteModel.Title.Value;
            ss.ReferenceType = "Issues";
            ss.ParentId = siteModel.ParentId;
            ss.InheritPermission = siteModel.InheritPermission;
            ss.AccessStatus = siteModel.AccessStatus;
            ss.Init();
            ss.SetLinkedSiteSettings();
            ss.SetPermissions(referenceId);
            ss.SetJoinedSsHash();
            if (setSiteIntegration) ss.SetSiteIntegration();
            ss.SetChoiceHash(all: setAllChoices);
            return ss;
        }

        public static SiteSettings IssuesSiteSettings(long siteId, bool setAllChoices = false)
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Issues";
            ss.SiteId = siteId;
            ss.Init();
            ss.SetChoiceHash(all: setAllChoices);
            return ss;
        }

        public static SiteSettings ResultsSiteSettings(
            this SiteModel siteModel,
            long referenceId,
            bool setSiteIntegration = false,
            bool setAllChoices = false)
        {
            var ss = siteModel.SiteSettings ?? new SiteSettings();
            ss.SiteId = siteModel.SiteId;
            ss.ReferenceId = referenceId;
            ss.Title = siteModel.Title.Value;
            ss.ReferenceType = "Results";
            ss.ParentId = siteModel.ParentId;
            ss.InheritPermission = siteModel.InheritPermission;
            ss.AccessStatus = siteModel.AccessStatus;
            ss.Init();
            ss.SetLinkedSiteSettings();
            ss.SetPermissions(referenceId);
            ss.SetJoinedSsHash();
            if (setSiteIntegration) ss.SetSiteIntegration();
            ss.SetChoiceHash(all: setAllChoices);
            return ss;
        }

        public static SiteSettings ResultsSiteSettings(long siteId, bool setAllChoices = false)
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Results";
            ss.SiteId = siteId;
            ss.Init();
            ss.SetChoiceHash(all: setAllChoices);
            return ss;
        }

        public static SiteSettings WikisSiteSettings(
            this SiteModel siteModel,
            long referenceId,
            bool setSiteIntegration = false,
            bool setAllChoices = false)
        {
            var ss = siteModel.SiteSettings ?? new SiteSettings();
            ss.SiteId = siteModel.SiteId;
            ss.ReferenceId = referenceId;
            ss.Title = siteModel.Title.Value;
            ss.ReferenceType = "Wikis";
            ss.ParentId = siteModel.ParentId;
            ss.InheritPermission = siteModel.InheritPermission;
            ss.AccessStatus = siteModel.AccessStatus;
            ss.Init();
            ss.SetLinkedSiteSettings();
            ss.SetPermissions(referenceId);
            ss.SetJoinedSsHash();
            ss.SetChoiceHash(all: setAllChoices);
            return ss;
        }

        public static SiteSettings WikisSiteSettings(long siteId, bool setAllChoices = false)
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Wikis";
            ss.SiteId = siteId;
            ss.Init();
            ss.SetChoiceHash(all: setAllChoices);
            return ss;
        }

        public static SiteSettings PermissionsSiteSettings(this SiteModel siteModel)
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Permissions";
            ss.SiteId = siteModel.SiteId;
            ss.InheritPermission = siteModel.InheritPermission;
            ss.ParentId = siteModel.ParentId;
            ss.Title = siteModel.Title.Value;
            ss.AccessStatus = siteModel.AccessStatus;
            ss.Init();
            return ss;
        }

        public static SiteSettings GetByDataRow(long siteId)
        {
            var dataRow = Rds.ExecuteTable(statements: Rds.SelectSites(
                column: Rds.SitesColumn()
                    .SiteSettings()
                    .Title()
                    .InheritPermission(),
                where: Rds.SitesWhere()
                    .TenantId(Sessions.TenantId())
                    .SiteId(siteId)))
                        .AsEnumerable()
                        .FirstOrDefault();
            if (dataRow != null)
            {
                var ss = dataRow
                    .String("SiteSettings")
                    .Deserialize<SiteSettings>() ?? new SiteSettings();
                ss.SiteId = siteId;
                ss.Title = dataRow.String("Title");
                ss.InheritPermission = dataRow.Long("InheritPermission");
                return ss;
            }
            else
            {
                return null;
            }
        }
    }
}
