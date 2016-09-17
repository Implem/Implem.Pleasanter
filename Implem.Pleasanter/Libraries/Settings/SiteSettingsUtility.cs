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

        public static SiteSettings Get(SiteModel siteModel)
        {
            switch (siteModel.ReferenceType)
            {
                case "Sites": return SitesSiteSettings(siteModel);
                case "Issues": return IssuesSiteSettings(siteModel);
                case "Results": return ResultsSiteSettings(siteModel);
                case "Wikis": return WikisSiteSettings(siteModel);
                default: return new SiteSettings() { SiteId = siteModel.SiteId };
            }
        }

        public static SiteSettings TenantsSiteSettings()
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "Tenants";
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings DemosSiteSettings()
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "Demos";
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings SysLogsSiteSettings()
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "SysLogs";
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings DeptsSiteSettings()
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "Depts";
            siteSettings.Init();
            siteSettings.SetChoicesByPlaceholders();
            return siteSettings;
        }

        public static SiteSettings UsersSiteSettings()
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "Users";
            siteSettings.Init();
            siteSettings.SetChoicesByPlaceholders();
            return siteSettings;
        }

        public static SiteSettings MailAddressesSiteSettings()
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "MailAddresses";
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings OutgoingMailsSiteSettings()
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "OutgoingMails";
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings SearchIndexesSiteSettings()
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "SearchIndexes";
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings ItemsSiteSettings()
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "Items";
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings OrdersSiteSettings()
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "Orders";
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings ExportSettingsSiteSettings()
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "ExportSettings";
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings LinksSiteSettings()
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "Links";
            siteSettings.Init();
            return siteSettings;
        }

        public static SiteSettings BinariesSiteSettings()
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "Binaries";
            siteSettings.Init();
            return siteSettings;
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
            siteSettings.SetChoicesByLinks();
            siteSettings.SetChoicesByPlaceholders();
            return siteSettings;
        }

        public static SiteSettings IssuesSiteSettings(long siteId)
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "Issues";
            siteSettings.SiteId = siteId;
            siteSettings.Init();
            siteSettings.SetChoicesByLinks();
            siteSettings.SetChoicesByPlaceholders();
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
            siteSettings.SetChoicesByLinks();
            siteSettings.SetChoicesByPlaceholders();
            return siteSettings;
        }

        public static SiteSettings ResultsSiteSettings(long siteId)
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "Results";
            siteSettings.SiteId = siteId;
            siteSettings.Init();
            siteSettings.SetChoicesByLinks();
            siteSettings.SetChoicesByPlaceholders();
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
            siteSettings.SetChoicesByLinks();
            siteSettings.SetChoicesByPlaceholders();
            return siteSettings;
        }

        public static SiteSettings WikisSiteSettings(long siteId)
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "Wikis";
            siteSettings.SiteId = siteId;
            siteSettings.Init();
            siteSettings.SetChoicesByLinks();
            siteSettings.SetChoicesByPlaceholders();
            return siteSettings;
        }

        public static SiteSettings PermissionsSiteSettings(this SiteModel siteModel)
        {
            var siteSettings = new SiteSettings();
            siteSettings.ReferenceType = "Permissions";
            siteSettings.SiteId = siteModel.SiteId;
            siteSettings.InheritPermission = siteModel.InheritPermission;
            siteSettings.ParentId = siteModel.ParentId;
            siteSettings.Title = siteModel.Title.Value;
            siteSettings.AccessStatus = siteModel.AccessStatus;
            siteSettings.Init();
            return siteSettings;
        }
    }
}
