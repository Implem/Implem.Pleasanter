using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class SiteSettingsUtilities
    {
        public const decimal Version = 1.016M;

        public static SiteSettings Get(
            Context context,
            long siteId,
            long referenceId = 0,
            bool setSiteIntegration = false,
            bool setAllChoices = false,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            return Get(
                context: context,
                siteModel: new SiteModel(context: context, siteId: siteId),
                referenceId: referenceId != 0
                    ? referenceId
                    : siteId,
                setSiteIntegration: setSiteIntegration,
                setAllChoices: setAllChoices,
                tableType: tableType);
        }

        public static SiteSettings Get(Context context, DataRow dataRow)
        {
            return dataRow != null
                ? dataRow.String("SiteSettings").DeserializeSiteSettings(context: context)
                    ?? Get(
                        context: context,
                        referenceType: dataRow.String("ReferenceType"),
                        siteId: dataRow.Long("SiteId"))
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

        public static SiteSettings Get(Context context, string referenceType, long siteId)
        {
            switch (referenceType)
            {
                case "Sites": return SitesSiteSettings(context: context, siteId: siteId);
                case "Issues": return IssuesSiteSettings(context: context, siteId: siteId);
                case "Results": return ResultsSiteSettings(context: context, siteId: siteId);
                case "Wikis": return WikisSiteSettings(context: context, siteId: siteId);
                default: return new SiteSettings() { SiteId = siteId };
            }
        }

        public static SiteSettings Get(
            Context context,
            SiteModel siteModel,
            long referenceId,
            bool setSiteIntegration = false,
            bool setAllChoices = false,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            switch (siteModel.ReferenceType)
            {
                case "Sites": return SitesSiteSettings(
                    context: context,
                    siteModel: siteModel,
                    referenceId: referenceId,
                    setSiteIntegration: setSiteIntegration,
                    setAllChoices: setAllChoices,
                    tableType: tableType);
                case "Issues": return IssuesSiteSettings(
                    context: context,
                    siteModel: siteModel,
                    referenceId: referenceId,
                    setSiteIntegration: setSiteIntegration,
                    setAllChoices: setAllChoices,
                    tableType: tableType);
                case "Results": return ResultsSiteSettings(
                    context: context,
                    siteModel: siteModel,
                    referenceId: referenceId,
                    setSiteIntegration: setSiteIntegration,
                    setAllChoices: setAllChoices,
                    tableType: tableType);
                case "Wikis": return WikisSiteSettings(
                    context: context,
                    siteModel: siteModel,
                    referenceId: referenceId,
                    setSiteIntegration: setSiteIntegration,
                    setAllChoices: setAllChoices,
                    tableType: tableType);
                default: return new SiteSettings() { SiteId = siteModel.SiteId };
            }
        }

        public static SiteSettings GetByReference(
            Context context, string reference, long referenceId)
        {
            switch (reference.ToLower())
            {
                case "tenants": return TenantsSiteSettings(context: context);
                case "demos": return DemosSiteSettings(context: context);
                case "sessions": return SessionsSiteSettings(context: context);
                case "syslogs": return SysLogsSiteSettings(context: context);
                case "statuses": return StatusesSiteSettings(context: context);
                case "reminderschedules": return ReminderSchedulesSiteSettings(context: context);
                case "depts": return DeptsSiteSettings(context: context);
                case "groups": return GroupsSiteSettings(context: context);
                case "groupmembers": return GroupMembersSiteSettings(context: context);
                case "registrations": return RegistrationsSiteSettings(context: context);
                case "users": return UsersSiteSettings(context: context);
                case "loginkeys": return LoginKeysSiteSettings(context: context);
                case "mailaddresses": return MailAddressesSiteSettings(context: context);
                case "outgoingmails": return OutgoingMailsSiteSettings(context: context);
                case "orders": return OrdersSiteSettings(context: context);
                case "exportsettings": return ExportSettingsSiteSettings(context: context);
                case "links": return LinksSiteSettings(context: context);
                case "binaries": return BinariesSiteSettings(context: context);
                case "items": return Get(
                    context: context,
                    siteModel: new ItemModel(
                        context: context,
                        referenceId: referenceId)
                            .GetSite(context: context),
                    referenceId: referenceId);
                default: return null;
            }
        }

        public static SiteSettings TenantsSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Tenants"
            };
            ss.Init(context: context);
            ss.SetChoiceHash(context: context, withLink: false);
            ss.PermissionType = Permissions.Admins(context: context);
            return ss;
        }

        public static SiteSettings DemosSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Demos"
            };
            ss.Init(context: context);
            return ss;
        }

        public static SiteSettings SessionsSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Sessions"
            };
            ss.Init(context: context);
            return ss;
        }

        public static SiteSettings SysLogsSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "SysLogs"
            };
            ss.Init(context: context);
            return ss;
        }

        public static SiteSettings StatusesSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Statuses"
            };
            ss.Init(context: context);
            return ss;
        }

        public static SiteSettings ReminderSchedulesSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "ReminderSchedules"
            };
            ss.Init(context: context);
            return ss;
        }

        public static SiteSettings DeptsSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Depts"
            };
            ss.Init(context: context);
            ss.SetChoiceHash(context: context, withLink: false);
            ss.PermissionType = Permissions.Admins(context: context);
            return ss;
        }

        public static SiteSettings GroupsSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Groups"
            };
            ss.Init(context: context);
            ss.SetChoiceHash(context: context, withLink: false);
            ss.PermissionType = Permissions.Admins(context: context);
            return ss;
        }

        public static SiteSettings GroupMembersSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "GroupMembers"
            };
            ss.Init(context: context);
            return ss;
        }

        public static SiteSettings RegistrationsSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Registrations"
            };
            ss.Init(context: context);
            ss.SetChoiceHash(context: context, withLink: false);
            ss.PermissionType = Permissions.Admins(context: context);
            return ss;
        }

        public static SiteSettings UsersSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Users"
            };
            ss.Init(context: context);
            ss.SetChoiceHash(context: context, withLink: false);
            ss.PermissionType = Permissions.Admins(context: context);
            return ss;
        }

        public static SiteSettings LoginKeysSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "LoginKeys"
            };
            ss.Init(context: context);
            return ss;
        }

        public static SiteSettings MailAddressesSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "MailAddresses"
            };
            ss.Init(context: context);
            return ss;
        }

        public static SiteSettings OutgoingMailsSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "OutgoingMails"
            };
            ss.Init(context: context);
            return ss;
        }

        public static SiteSettings ItemsSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Items"
            };
            ss.Init(context: context);
            return ss;
        }

        public static SiteSettings OrdersSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Orders"
            };
            ss.Init(context: context);
            return ss;
        }

        public static SiteSettings ExportSettingsSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "ExportSettings"
            };
            ss.Init(context: context);
            return ss;
        }

        public static SiteSettings LinksSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Links"
            };
            ss.Init(context: context);
            return ss;
        }

        public static SiteSettings BinariesSiteSettings(Context context)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Binaries"
            };
            ss.Init(context: context);
            return ss;
        }

        public static SiteSettings SitesSiteSettings(
            this SiteModel siteModel,
            Context context,
            long referenceId,
            bool setSiteIntegration = false,
            bool setAllChoices = false,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            var ss = siteModel.SiteSettings ?? new SiteSettings();
            ss.LockedTableTime = siteModel.LockedTime;
            ss.LockedTableUser = siteModel.LockedUser;
            ss.TableType = tableType;
            ss.SiteId = siteModel.SiteId;
            ss.ReferenceId = referenceId;
            ss.Title = siteModel.Title.Value;
            ss.Body = siteModel.Body;
            ss.GridGuide = siteModel.GridGuide;
            ss.EditorGuide = siteModel.EditorGuide;
            ss.ReferenceType = "Sites";
            ss.ParentId = siteModel.ParentId;
            ss.InheritPermission = siteModel.InheritPermission;
            ss.Publish = siteModel.Publish;
            ss.AccessStatus = siteModel.AccessStatus;
            ss.ApiCount = siteModel.ApiCount;
            ss.ApiCountDate = siteModel.ApiCountDate;
            ss.Init(context: context);
            ss.SetLinkedSiteSettings(context: context);
            ss.SetPermissions(context: context, referenceId: referenceId);
            if (setSiteIntegration) ss.SetSiteIntegration(context: context);
            return ss;
        }

        public static SiteSettings SitesSiteSettings(
            Context context, long siteId, bool setAllChoices = false)
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Sites";
            ss.SiteId = siteId;
            ss.Init(context: context);
            return ss;
        }

        public static SiteSettings IssuesSiteSettings(
            this SiteModel siteModel,
            Context context,
            long referenceId,
            bool setSiteIntegration = false,
            bool setAllChoices = false,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            var ss = siteModel.SiteSettings ?? new SiteSettings();
            ss.LockedTableTime = siteModel.LockedTime;
            ss.LockedTableUser = siteModel.LockedUser;
            ss.TableType = tableType;
            ss.SiteId = siteModel.SiteId;
            ss.ReferenceId = referenceId;
            ss.Title = siteModel.Title.Value;
            ss.Body = siteModel.Body;
            ss.GridGuide = siteModel.GridGuide;
            ss.EditorGuide = siteModel.EditorGuide;
            ss.ReferenceType = "Issues";
            ss.ParentId = siteModel.ParentId;
            ss.InheritPermission = siteModel.InheritPermission;
            ss.Publish = siteModel.Publish;
            ss.AccessStatus = siteModel.AccessStatus;
            ss.ApiCount = siteModel.ApiCount;
            ss.ApiCountDate = siteModel.ApiCountDate;
            ss.Init(context: context);
            ss.SetLinkedSiteSettings(context: context);
            ss.SetPermissions(context: context, referenceId: referenceId);
            if (setSiteIntegration) ss.SetSiteIntegration(context: context);
            ss.SetChoiceHash(context: context, all: setAllChoices);
            return ss;
        }

        public static SiteSettings IssuesSiteSettings(
            Context context, long siteId, bool setAllChoices = false)
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Issues";
            ss.SiteId = siteId;
            ss.Init(context: context);
            ss.SetChoiceHash(context: context, all: setAllChoices);
            return ss;
        }

        public static SiteSettings ResultsSiteSettings(
            this SiteModel siteModel,
            Context context,
            long referenceId,
            bool setSiteIntegration = false,
            bool setAllChoices = false,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            var ss = siteModel.SiteSettings ?? new SiteSettings();
            ss.LockedTableTime = siteModel.LockedTime;
            ss.LockedTableUser = siteModel.LockedUser;
            ss.TableType = tableType;
            ss.SiteId = siteModel.SiteId;
            ss.ReferenceId = referenceId;
            ss.Title = siteModel.Title.Value;
            ss.Body = siteModel.Body;
            ss.GridGuide = siteModel.GridGuide;
            ss.EditorGuide = siteModel.EditorGuide;
            ss.ReferenceType = "Results";
            ss.ParentId = siteModel.ParentId;
            ss.InheritPermission = siteModel.InheritPermission;
            ss.Publish = siteModel.Publish;
            ss.AccessStatus = siteModel.AccessStatus;
            ss.ApiCount = siteModel.ApiCount;
            ss.ApiCountDate = siteModel.ApiCountDate;
            ss.Init(context: context);
            ss.SetLinkedSiteSettings(context: context);
            ss.SetPermissions(context: context, referenceId: referenceId);
            if (setSiteIntegration) ss.SetSiteIntegration(context: context);
            ss.SetChoiceHash(context: context, all: setAllChoices);
            return ss;
        }

        public static SiteSettings ResultsSiteSettings(
            Context context, long siteId, bool setAllChoices = false)
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Results";
            ss.SiteId = siteId;
            ss.Init(context: context);
            ss.SetChoiceHash(context: context, all: setAllChoices);
            return ss;
        }

        public static SiteSettings WikisSiteSettings(
            this SiteModel siteModel,
            Context context,
            long referenceId,
            bool setSiteIntegration = false,
            bool setAllChoices = false,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            var ss = siteModel.SiteSettings ?? new SiteSettings();
            ss.LockedTableTime = siteModel.LockedTime;
            ss.LockedTableUser = siteModel.LockedUser;
            ss.TableType = tableType;
            ss.SiteId = siteModel.SiteId;
            ss.ReferenceId = referenceId;
            ss.Title = siteModel.Title.Value;
            ss.Body = siteModel.Body;
            ss.GridGuide = siteModel.GridGuide;
            ss.EditorGuide = siteModel.EditorGuide;
            ss.ReferenceType = "Wikis";
            ss.ParentId = siteModel.ParentId;
            ss.InheritPermission = siteModel.InheritPermission;
            ss.Publish = siteModel.Publish;
            ss.AccessStatus = siteModel.AccessStatus;
            ss.ApiCount = siteModel.ApiCount;
            ss.ApiCountDate = siteModel.ApiCountDate;
            ss.Init(context: context);
            ss.SetLinkedSiteSettings(context: context);
            ss.SetPermissions(context: context, referenceId: referenceId);
            ss.SetChoiceHash(context: context, all: setAllChoices);
            return ss;
        }

        public static SiteSettings WikisSiteSettings(
            Context context, long siteId, bool setAllChoices = false)
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Wikis";
            ss.SiteId = siteId;
            ss.Init(context: context);
            ss.SetChoiceHash(context: context, all: setAllChoices);
            return ss;
        }

        public static SiteSettings PermissionsSiteSettings(
            this SiteModel siteModel, Context context)
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Permissions";
            ss.SiteId = siteModel.SiteId;
            ss.InheritPermission = siteModel.InheritPermission;
            ss.ParentId = siteModel.ParentId;
            ss.Title = siteModel.Title.Value;
            ss.Body = siteModel.Body;
            ss.AccessStatus = siteModel.AccessStatus;
            ss.Init(context: context);
            return ss;
        }

        public static SiteSettings GetByDataRow(Context context, long siteId)
        {
            var dataRow = Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn()
                        .SiteSettings()
                        .Title()
                        .InheritPermission(),
                    where: Rds.SitesWhere()
                        .TenantId(context.TenantId)
                        .SiteId(siteId)))
                            .AsEnumerable()
                            .FirstOrDefault();
            if (dataRow != null)
            {
                var ss = dataRow
                    .String("SiteSettings")
                    .DeserializeSiteSettings(context: context) ?? new SiteSettings();
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SiteSettings DeserializeSiteSettings(this string json, Context context)
        {
            var ss = !json.IsNullOrEmpty()
                ? json.Deserialize<SiteSettings>()
                : null;
            if (ss != null)
            {
                if (ss.Version != Version)
                {
                    Migrators.SiteSettingsMigrator.Migrate(ss);
                }
                ss.Init(context: context);
            }
            return ss;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SiteSettings ApiUsersSiteSettings(Context context)
        {
            var ss = UsersSiteSettings(context);
            ss?.Columns?
                .Where(c => c.Name == "Disabled")?
                .ForEach(c => c.CheckFilterControlType = ColumnUtilities.CheckFilterControlTypes.OnAndOff);
            return ss;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SiteSettings ApiGroupsSiteSettings(Context context)
        {
            var ss = GroupsSiteSettings(context);
            ss?.Columns?
                .Where(c => c.Name == "Disabled")?
                .ForEach(c => c.CheckFilterControlType = ColumnUtilities.CheckFilterControlTypes.OnAndOff);
            return ss;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SiteSettings ApiDeptsSiteSettings(Context context)
        {
            var ss = DeptsSiteSettings(context);
            ss?.Columns?
                .Where(c => c.Name == "Disabled")?
                .ForEach(c => c.CheckFilterControlType = ColumnUtilities.CheckFilterControlTypes.OnAndOff);
            return ss;
        }
    }
}
