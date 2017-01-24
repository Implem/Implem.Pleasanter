using Implem.Libraries.Utilities;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class SiteSettingsUtilities
    {
        public static SiteSettings Get(DataRow dataRow)
        {
            return dataRow != null
                ? dataRow["SiteSettings"]
                    .ToString()
                    .Deserialize<SiteSettings>() ??
                        Get(dataRow["SiteId"].ToLong(),
                            dataRow["ReferenceType"].ToString())
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

        public static SiteSettings DeptsSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Depts";
            ss.Init();
            ss.SetChoiceHash(withLink: false);
            return ss;
        }

        public static SiteSettings UsersSiteSettings()
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Users";
            ss.Init();
            ss.SetChoiceHash(withLink: false);
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

        public static SiteSettings SitesSiteSettings(this SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings ?? new SiteSettings();
            ss.SiteId = siteModel.SiteId;
            ss.Title = siteModel.Title.Value;
            ss.ReferenceType = "Sites";
            ss.ParentId = siteModel.ParentId;
            ss.InheritPermission = siteModel.InheritPermission;
            ss.PermissionType = siteModel.PermissionType;
            ss.AccessStatus = siteModel.AccessStatus;
            ss.Init();
            ss.SetLinkedSiteSettings();
            return ss;
        }

        public static SiteSettings SitesSiteSettings(long siteId)
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Sites";
            ss.SiteId = siteId;
            ss.Init();
            return ss;
        }

        public static SiteSettings IssuesSiteSettings(this SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings ?? new SiteSettings();
            ss.SiteId = siteModel.SiteId;
            ss.Title = siteModel.Title.Value;
            ss.ReferenceType = "Issues";
            ss.ParentId = siteModel.ParentId;
            ss.InheritPermission = siteModel.InheritPermission;
            ss.PermissionType = siteModel.PermissionType;
            ss.AccessStatus = siteModel.AccessStatus;
            ss.Init();
            ss.SetLinkedSiteSettings();
            ss.SetChoiceHash();
            return ss;
        }

        public static SiteSettings IssuesSiteSettings(long siteId)
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Issues";
            ss.SiteId = siteId;
            ss.Init();
            ss.SetChoiceHash();
            return ss;
        }

        public static SiteSettings ResultsSiteSettings(this SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings ?? new SiteSettings();
            ss.SiteId = siteModel.SiteId;
            ss.Title = siteModel.Title.Value;
            ss.ReferenceType = "Results";
            ss.ParentId = siteModel.ParentId;
            ss.InheritPermission = siteModel.InheritPermission;
            ss.PermissionType = siteModel.PermissionType;
            ss.AccessStatus = siteModel.AccessStatus;
            ss.Init();
            ss.SetLinkedSiteSettings();
            ss.SetChoiceHash();
            return ss;
        }

        public static SiteSettings ResultsSiteSettings(long siteId)
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Results";
            ss.SiteId = siteId;
            ss.Init();
            ss.SetChoiceHash();
            return ss;
        }

        public static SiteSettings WikisSiteSettings(this SiteModel siteModel)
        {
            var ss = siteModel.SiteSettings ?? new SiteSettings();
            ss.SiteId = siteModel.SiteId;
            ss.Title = siteModel.Title.Value;
            ss.ReferenceType = "Wikis";
            ss.ParentId = siteModel.ParentId;
            ss.InheritPermission = siteModel.InheritPermission;
            ss.PermissionType = siteModel.PermissionType;
            ss.AccessStatus = siteModel.AccessStatus;
            ss.Init();
            ss.SetLinkedSiteSettings();
            ss.SetChoiceHash();
            return ss;
        }

        public static SiteSettings WikisSiteSettings(long siteId)
        {
            var ss = new SiteSettings();
            ss.ReferenceType = "Wikis";
            ss.SiteId = siteId;
            ss.Init();
            ss.SetChoiceHash();
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
    }
}
