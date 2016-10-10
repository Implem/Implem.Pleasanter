using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class SiteValidators
    {
        public static Error.Types OnCreating(
            SiteSettings siteSettings, Permissions.Types permissionType, SiteModel siteModel)
        {
            if (!permissionType.CanEditSite())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Sites_TenantId":
                        if (!siteSettings.GetColumn("TenantId").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_SiteId":
                        if (!siteSettings.GetColumn("SiteId").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_UpdatedTime":
                        if (!siteSettings.GetColumn("UpdatedTime").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Ver":
                        if (!siteSettings.GetColumn("Ver").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Title":
                        if (!siteSettings.GetColumn("Title").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Body":
                        if (!siteSettings.GetColumn("Body").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_TitleBody":
                        if (!siteSettings.GetColumn("TitleBody").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_ReferenceType":
                        if (!siteSettings.GetColumn("ReferenceType").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_ParentId":
                        if (!siteSettings.GetColumn("ParentId").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_InheritPermission":
                        if (!siteSettings.GetColumn("InheritPermission").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_PermissionType":
                        if (!siteSettings.GetColumn("PermissionType").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_SiteSettings":
                        if (!siteSettings.GetColumn("SiteSettings").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Ancestors":
                        if (!siteSettings.GetColumn("Ancestors").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_PermissionSourceCollection":
                        if (!siteSettings.GetColumn("PermissionSourceCollection").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_PermissionDestinationCollection":
                        if (!siteSettings.GetColumn("PermissionDestinationCollection").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_SiteMenu":
                        if (!siteSettings.GetColumn("SiteMenu").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_MonitorChangesColumns":
                        if (!siteSettings.GetColumn("MonitorChangesColumns").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_TitleColumns":
                        if (!siteSettings.GetColumn("TitleColumns").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Comments":
                        if (!siteSettings.GetColumn("Comments").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Creator":
                        if (!siteSettings.GetColumn("Creator").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Updator":
                        if (!siteSettings.GetColumn("Updator").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_CreatedTime":
                        if (!siteSettings.GetColumn("CreatedTime").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_VerUp":
                        if (!siteSettings.GetColumn("VerUp").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Timestamp":
                        if (!siteSettings.GetColumn("Timestamp").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(
            SiteSettings siteSettings, Permissions.Types permissionType, SiteModel siteModel)
        {
            if (!permissionType.CanEditSite())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Sites_TenantId":
                        if (!siteSettings.GetColumn("TenantId").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_SiteId":
                        if (!siteSettings.GetColumn("SiteId").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_UpdatedTime":
                        if (!siteSettings.GetColumn("UpdatedTime").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Ver":
                        if (!siteSettings.GetColumn("Ver").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Title":
                        if (!siteSettings.GetColumn("Title").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Body":
                        if (!siteSettings.GetColumn("Body").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_TitleBody":
                        if (!siteSettings.GetColumn("TitleBody").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_ReferenceType":
                        if (!siteSettings.GetColumn("ReferenceType").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_ParentId":
                        if (!siteSettings.GetColumn("ParentId").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_InheritPermission":
                        if (!siteSettings.GetColumn("InheritPermission").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_PermissionType":
                        if (!siteSettings.GetColumn("PermissionType").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_SiteSettings":
                        if (!siteSettings.GetColumn("SiteSettings").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Ancestors":
                        if (!siteSettings.GetColumn("Ancestors").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_PermissionSourceCollection":
                        if (!siteSettings.GetColumn("PermissionSourceCollection").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_PermissionDestinationCollection":
                        if (!siteSettings.GetColumn("PermissionDestinationCollection").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_SiteMenu":
                        if (!siteSettings.GetColumn("SiteMenu").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_MonitorChangesColumns":
                        if (!siteSettings.GetColumn("MonitorChangesColumns").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_TitleColumns":
                        if (!siteSettings.GetColumn("TitleColumns").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Comments":
                        if (!siteSettings.GetColumn("Comments").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Creator":
                        if (!siteSettings.GetColumn("Creator").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Updator":
                        if (!siteSettings.GetColumn("Updator").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_CreatedTime":
                        if (!siteSettings.GetColumn("CreatedTime").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_VerUp":
                        if (!siteSettings.GetColumn("VerUp").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Sites_Timestamp":
                        if (!siteSettings.GetColumn("Timestamp").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(
            SiteSettings siteSettings, Permissions.Types permissionType, SiteModel siteModel)
        {
            if (!permissionType.CanEditSite())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnRestoring()
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}
