using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
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
        public const decimal Version = 1.017M;

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

        public static Section Get(this List<Section> sections, int? id)
        {
            return sections?.FirstOrDefault(o => o.Id == id);
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
                case "Dashboards": return DashboardsSiteSettings(context: context, siteId: siteId);
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
                case "Dashboards": return DashboardsSiteSettings(
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
                case "autonumberings": return AutoNumberingsSiteSettings(context: context);
                case "binaries": return BinariesSiteSettings(context: context);
                case "demos": return DemosSiteSettings(context: context);
                case "depts": return DeptsSiteSettings(context: context);
                case "exportsettings": return ExportSettingsSiteSettings(context: context);
                case "extensions": return ExtensionsSiteSettings(context: context);
                case "groupchildren": return GroupChildrenSiteSettings(context: context);
                case "groupmembers": return GroupMembersSiteSettings(context: context);
                case "groups": return GroupsSiteSettings(context: context);
                case "links": return LinksSiteSettings(context: context);
                case "loginkeys": return LoginKeysSiteSettings(context: context);
                case "mailaddresses": return MailAddressesSiteSettings(context: context);
                case "orders": return OrdersSiteSettings(context: context);
                case "outgoingmails": return OutgoingMailsSiteSettings(context: context);
                case "registrations": return RegistrationsSiteSettings(context: context);
                case "reminderschedules": return ReminderSchedulesSiteSettings(context: context);
                case "sessions": return SessionsSiteSettings(context: context);
                case "statuses": return StatusesSiteSettings(context: context);
                case "syslogs": return SysLogsSiteSettings(context: context);
                case "tenants": return TenantsSiteSettings(context: context);
                case "users": return UsersSiteSettings(context: context);
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

        public static SiteSettings AutoNumberingsSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "AutoNumberings"
            };
            ss.Init(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings BinariesSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Binaries"
            };
            ss.Init(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings DemosSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Demos"
            };
            ss.Init(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings DeptsSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Depts"
            };
            ss.Init(context: context);
            ss.SetLinks(context: context);
            ss.SetChoiceHash(context: context, withLink: false);
            ss.PermissionType = Permissions.Admins(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings ExportSettingsSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "ExportSettings"
            };
            ss.Init(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings ExtensionsSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Extensions"
            };
            ss.Init(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings GroupChildrenSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "GroupChildren"
            };
            ss.Init(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings GroupMembersSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "GroupMembers"
            };
            ss.Init(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings GroupsSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Groups"
            };
            ss.Init(context: context);
            ss.SetLinks(context: context);
            ss.SetChoiceHash(context: context, withLink: false);
            ss.PermissionType = Permissions.Admins(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings ItemsSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Items"
            };
            ss.Init(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings LinksSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Links"
            };
            ss.Init(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings LoginKeysSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "LoginKeys"
            };
            ss.Init(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings MailAddressesSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "MailAddresses"
            };
            ss.Init(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings OrdersSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Orders"
            };
            ss.Init(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings OutgoingMailsSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "OutgoingMails"
            };
            ss.Init(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings RegistrationsSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Registrations"
            };
            ss.Init(context: context);
            ss.SetLinks(context: context);
            ss.SetChoiceHash(context: context, withLink: false);
            ss.PermissionType = Permissions.Admins(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings ReminderSchedulesSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "ReminderSchedules"
            };
            ss.Init(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings SessionsSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Sessions"
            };
            ss.Init(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings StatusesSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Statuses"
            };
            ss.Init(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings SysLogsSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "SysLogs"
            };
            ss.Init(context: context);
            ss.SetLinks(context: context);
            ss.SetChoiceHash(context: context, withLink: false);
            ss.PermissionType = Permissions.Admins(context: context);
            ss.UseFilterButton = true;
            ss.AlwaysRequestSearchCondition = true;
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings TenantsSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Tenants"
            };
            ss.Init(context: context);
            ss.SetLinks(context: context);
            ss.SetChoiceHash(context: context, withLink: false);
            ss.PermissionType = Permissions.Admins(context: context);
            ss.TableType = tableTypes;
            return ss;
        }

        public static SiteSettings UsersSiteSettings(Context context, Sqls.TableTypes tableTypes = Sqls.TableTypes.Normal)
        {
            var ss = new SiteSettings()
            {
                ReferenceType = "Users"
            };
            ss.Init(context: context);
            ss.SetLinks(context: context);
            ss.SetChoiceHash(context: context, withLink: false);
            ss.PermissionType = Permissions.Admins(context: context);
            ss.TableType = tableTypes;
            // ContractSettingsでAPIが無効化されている場合は無条件で使用できなくする
            // APIを許可の項目は、APIの無効化が設定されていない場合には使用できなくする
            // APIの無効化はUser.jsonまたはテナントの管理の何れかで設定する
            if (context.ContractSettings.Api == false
                || (!DefinitionAccessor.Parameters.User.DisableApi && !context.DisableApi))
            {
                var column = ss.GetColumn(
                    context: context,
                    columnName: "AllowApi");
                var columnAccessControl = new ColumnAccessControl()
                {
                    No = column.No,
                    ColumnName = column.ColumnName,
                    Type = Permissions.Types.ManageService
                };
                ss.CreateColumnAccessControls = ss.CreateColumnAccessControls ?? new List<ColumnAccessControl>();
                ss.ReadColumnAccessControls = ss.ReadColumnAccessControls ?? new List<ColumnAccessControl>();
                ss.UpdateColumnAccessControls = ss.UpdateColumnAccessControls ?? new List<ColumnAccessControl>();
                ss.CreateColumnAccessControls.Add(columnAccessControl);
                ss.ReadColumnAccessControls.Add(columnAccessControl);
                ss.UpdateColumnAccessControls.Add(columnAccessControl);
            }
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
            ss.CalendarGuide = siteModel.CalendarGuide;
            ss.CrosstabGuide = siteModel.CrosstabGuide;
            ss.GanttGuide = siteModel.GanttGuide;
            ss.BurnDownGuide = siteModel.BurnDownGuide;
            ss.TimeSeriesGuide = siteModel.TimeSeriesGuide;
            ss.AnalyGuide = siteModel.AnalyGuide;
            ss.KambanGuide = siteModel.KambanGuide;
            ss.ImageLibGuide = siteModel.ImageLibGuide;
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

        public static SiteSettings DashboardsSiteSettings(
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
            ss.CalendarGuide = siteModel.CalendarGuide;
            ss.CrosstabGuide = siteModel.CrosstabGuide;
            ss.GanttGuide = siteModel.GanttGuide;
            ss.BurnDownGuide = siteModel.BurnDownGuide;
            ss.TimeSeriesGuide = siteModel.TimeSeriesGuide;
            ss.AnalyGuide = siteModel.AnalyGuide;
            ss.KambanGuide = siteModel.KambanGuide;
            ss.ImageLibGuide = siteModel.ImageLibGuide;
            ss.ReferenceType = "Dashboards";
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

        public static SiteSettings DashboardsSiteSettings(
            Context context, long siteId, bool setAllChoices = false)
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Dashboards";
            ss.SiteId = siteId;
            ss.Init(context: context);
            ss.SetChoiceHash(context: context, all: setAllChoices);
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
            ss.CalendarGuide = siteModel.CalendarGuide;
            ss.CrosstabGuide = siteModel.CrosstabGuide;
            ss.GanttGuide = siteModel.GanttGuide;
            ss.BurnDownGuide = siteModel.BurnDownGuide;
            ss.TimeSeriesGuide = siteModel.TimeSeriesGuide;
            ss.AnalyGuide = siteModel.AnalyGuide;
            ss.KambanGuide = siteModel.KambanGuide;
            ss.ImageLibGuide = siteModel.ImageLibGuide;
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
            ss.CalendarGuide = siteModel.CalendarGuide;
            ss.CrosstabGuide = siteModel.CrosstabGuide;
            ss.GanttGuide = siteModel.GanttGuide;
            ss.BurnDownGuide = siteModel.BurnDownGuide;
            ss.TimeSeriesGuide = siteModel.TimeSeriesGuide;
            ss.AnalyGuide = siteModel.AnalyGuide;
            ss.KambanGuide = siteModel.KambanGuide;
            ss.ImageLibGuide = siteModel.ImageLibGuide;
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
            ss.CalendarGuide = siteModel.CalendarGuide;
            ss.CrosstabGuide = siteModel.CrosstabGuide;
            ss.GanttGuide = siteModel.GanttGuide;
            ss.BurnDownGuide = siteModel.BurnDownGuide;
            ss.TimeSeriesGuide = siteModel.TimeSeriesGuide;
            ss.AnalyGuide = siteModel.AnalyGuide;
            ss.KambanGuide = siteModel.KambanGuide;
            ss.ImageLibGuide = siteModel.ImageLibGuide;
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
