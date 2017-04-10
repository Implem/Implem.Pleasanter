using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class SiteValidators
    {
        public static Error.Types OnCreating(SiteSettings ss, SiteModel siteModel)
        {
            if (!ss.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(siteModel.Mine());
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Sites_Title":
                        if (!ss.GetColumn("Title").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Sites_Body":
                        if (!ss.GetColumn("Body").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Sites_ReferenceType":
                        if (!ss.GetColumn("ReferenceType").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Sites_InheritPermission":
                        if (!ss.GetColumn("InheritPermission").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn("Comments").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(SiteSettings ss, SiteModel siteModel)
        {
            if (!ss.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(siteModel.Mine());
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Sites_Title":
                        if (siteModel.Title_Updated &&
                            !ss.GetColumn("Title").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Sites_Body":
                        if (siteModel.Body_Updated &&
                            !ss.GetColumn("Body").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Sites_ReferenceType":
                        if (siteModel.ReferenceType_Updated &&
                            !ss.GetColumn("ReferenceType").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Sites_InheritPermission":
                        if (siteModel.InheritPermission_Updated &&
                            !ss.GetColumn("InheritPermission").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "SearchPermissionElements":
                    case "OpenPermissionsDialog":
                    case "AddPermissions":
                    case "DeletePermissions":
                    case "InheritPermission":
                        if (!ss.CanManagePermission())
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn("Comments").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(SiteSettings ss, SiteModel siteModel)
        {
            if (!ss.CanDelete())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnRestoring()
        {
            if (!Permissions.CanManageTenant())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnExporting(SiteSettings ss)
        {
            if (!ss.CanExport())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnMoving(
            long currentId,
            long destinationId,
            SiteSettings current,
            SiteSettings source,
            SiteSettings destination)
        {
            if (currentId != 0 && !current.CanManageSite())
            {
                return Error.Types.HasNotPermission;
            }
            if (!source.CanManageSite())
            {
                return Error.Types.HasNotPermission;
            }
            if (destinationId != 0 && !destination.CanManageSite())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnSorting(SiteSettings ss)
        {
            if (ss.SiteId != 0 && !ss.CanManageSite())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnSetSiteSettings(SiteSettings ss, out string data)
        {
            data = null;
            if (!ss.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            foreach (var formData in Forms.All())
            {
                switch (formData.Key)
                {
                    case "Format":
                        try
                        {
                            0.ToString(formData.Value, Sessions.CultureInfo());
                        }
                        catch (System.Exception)
                        {
                            data = formData.Value;
                            return Error.Types.BadFormat;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }
    }
}
