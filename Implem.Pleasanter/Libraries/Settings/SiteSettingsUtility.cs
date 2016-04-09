using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class SiteSettingsUtility
    {
        public static SiteSettings Get(long siteId, string referenceType)
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

        public static SiteSettings TenantsSiteSettings()
        {
            var tenantsSiteSettings = new SiteSettings();
            tenantsSiteSettings.ReferenceType = "Tenants";
            tenantsSiteSettings.Init();
            return tenantsSiteSettings;
        }

        public static SiteSettings SysLogsSiteSettings()
        {
            var sysLogsSiteSettings = new SiteSettings();
            sysLogsSiteSettings.ReferenceType = "SysLogs";
            sysLogsSiteSettings.Init();
            return sysLogsSiteSettings;
        }

        public static SiteSettings DeptsSiteSettings()
        {
            var deptsSiteSettings = new SiteSettings();
            deptsSiteSettings.ReferenceType = "Depts";
            deptsSiteSettings.Init();
            return deptsSiteSettings;
        }

        public static SiteSettings UsersSiteSettings()
        {
            var usersSiteSettings = new SiteSettings();
            usersSiteSettings.ReferenceType = "Users";
            usersSiteSettings.Init();
            return usersSiteSettings;
        }

        public static SiteSettings MailAddressesSiteSettings()
        {
            var mailAddressesSiteSettings = new SiteSettings();
            mailAddressesSiteSettings.ReferenceType = "MailAddresses";
            mailAddressesSiteSettings.Init();
            return mailAddressesSiteSettings;
        }

        public static SiteSettings OutgoingMailsSiteSettings()
        {
            var outgoingMailsSiteSettings = new SiteSettings();
            outgoingMailsSiteSettings.ReferenceType = "OutgoingMails";
            outgoingMailsSiteSettings.Init();
            return outgoingMailsSiteSettings;
        }

        public static SiteSettings SearchIndexesSiteSettings()
        {
            var searchIndexesSiteSettings = new SiteSettings();
            searchIndexesSiteSettings.ReferenceType = "SearchIndexes";
            searchIndexesSiteSettings.Init();
            return searchIndexesSiteSettings;
        }

        public static SiteSettings ItemsSiteSettings()
        {
            var itemsSiteSettings = new SiteSettings();
            itemsSiteSettings.ReferenceType = "Items";
            itemsSiteSettings.Init();
            return itemsSiteSettings;
        }

        public static SiteSettings OrdersSiteSettings()
        {
            var ordersSiteSettings = new SiteSettings();
            ordersSiteSettings.ReferenceType = "Orders";
            ordersSiteSettings.Init();
            return ordersSiteSettings;
        }

        public static SiteSettings ExportSettingsSiteSettings()
        {
            var exportSettingsSiteSettings = new SiteSettings();
            exportSettingsSiteSettings.ReferenceType = "ExportSettings";
            exportSettingsSiteSettings.Init();
            return exportSettingsSiteSettings;
        }

        public static SiteSettings LinksSiteSettings()
        {
            var linksSiteSettings = new SiteSettings();
            linksSiteSettings.ReferenceType = "Links";
            linksSiteSettings.Init();
            return linksSiteSettings;
        }

        public static SiteSettings BinariesSiteSettings()
        {
            var binariesSiteSettings = new SiteSettings();
            binariesSiteSettings.ReferenceType = "Binaries";
            binariesSiteSettings.Init();
            return binariesSiteSettings;
        }

        public static SiteSettings SitesSiteSettings(this SiteModel siteModel)
        {
            var siteSettings = siteModel.SiteSettings ?? new SiteSettings();
            siteSettings.ReferenceType = "Sites";
            siteSettings.SiteId = siteModel.SiteId;
            siteSettings.InheritPermission = siteModel.InheritPermission;
            siteSettings.ParentId = siteModel.ParentId;
            siteSettings.Title = siteModel.Title.Value;
            siteSettings.AccessStatus = siteModel.AccessStatus;
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings SitesSiteSettings(long siteId)
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "Sites";
            siteSettings.SiteId = siteId;
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings IssuesSiteSettings(this SiteModel siteModel)
        {
            var siteSettings = siteModel.SiteSettings ?? new SiteSettings();
            siteSettings.ReferenceType = "Issues";
            siteSettings.SiteId = siteModel.SiteId;
            siteSettings.InheritPermission = siteModel.InheritPermission;
            siteSettings.ParentId = siteModel.ParentId;
            siteSettings.Title = siteModel.Title.Value;
            siteSettings.AccessStatus = siteModel.AccessStatus;
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings IssuesSiteSettings(long siteId)
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "Issues";
            siteSettings.SiteId = siteId;
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings ResultsSiteSettings(this SiteModel siteModel)
        {
            var siteSettings = siteModel.SiteSettings ?? new SiteSettings();
            siteSettings.ReferenceType = "Results";
            siteSettings.SiteId = siteModel.SiteId;
            siteSettings.InheritPermission = siteModel.InheritPermission;
            siteSettings.ParentId = siteModel.ParentId;
            siteSettings.Title = siteModel.Title.Value;
            siteSettings.AccessStatus = siteModel.AccessStatus;
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings ResultsSiteSettings(long siteId)
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "Results";
            siteSettings.SiteId = siteId;
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings WikisSiteSettings(this SiteModel siteModel)
        {
            var siteSettings = siteModel.SiteSettings ?? new SiteSettings();
            siteSettings.ReferenceType = "Wikis";
            siteSettings.SiteId = siteModel.SiteId;
            siteSettings.InheritPermission = siteModel.InheritPermission;
            siteSettings.ParentId = siteModel.ParentId;
            siteSettings.Title = siteModel.Title.Value;
            siteSettings.AccessStatus = siteModel.AccessStatus;
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings WikisSiteSettings(long siteId)
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "Wikis";
            siteSettings.SiteId = siteId;
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings PermissionsSiteSettings(this SiteModel siteModel)
        {
            var permissionsSiteSettings = new SiteSettings();
            permissionsSiteSettings.ReferenceType = "Permissions";
            permissionsSiteSettings.SiteId = siteModel.SiteId;
            permissionsSiteSettings.InheritPermission = siteModel.InheritPermission;
            permissionsSiteSettings.ParentId = siteModel.ParentId;
            permissionsSiteSettings.Title = siteModel.Title.Value;
            permissionsSiteSettings.AccessStatus = siteModel.AccessStatus;
            permissionsSiteSettings.Init();
            return permissionsSiteSettings;
        }
    }
}
