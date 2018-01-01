using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class SiteValidators
    {
        public static Error.Types OnEntry(SiteSettings ss)
        {
            return ss.HasPermission()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnReading(SiteSettings ss)
        {
            return ss.CanManageSite()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnEditing(SiteSettings ss, SiteModel siteModel)
        {
            return
                ss.CanManageSite() &&
                siteModel.AccessStatus != Databases.AccessStatuses.NotFound
                    ? Error.Types.None
                    : siteModel.MethodType == BaseModel.MethodTypes.New
                        ? Error.Types.HasNotPermission
                        : Error.Types.NotFound;
        }

        public static Error.Types OnCreating(SiteSettings ss, SiteModel siteModel)
        {
            if (!ss.CanManageSite())
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
            if (!ss.CanManageSite())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(siteModel.Mine());
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Sites_Title":
                        if (siteModel.Title_Updated() &&
                            !ss.GetColumn("Title").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Sites_Body":
                        if (siteModel.Body_Updated() &&
                            !ss.GetColumn("Body").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Sites_ReferenceType":
                        if (siteModel.ReferenceType_Updated() &&
                            !ss.GetColumn("ReferenceType").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Sites_InheritPermission":
                        if (siteModel.InheritPermission_Updated() &&
                            !ss.GetColumn("InheritPermission").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "InheritPermission":
                        var type = InheritPermission(ss);
                        if (type != Error.Types.None) return type;
                        break;
                    case "CurrentPermissionsAll":
                        if (!ss.CanManagePermission())
                        {
                            return Error.Types.HasNotPermission;
                        }
                        if (!new PermissionCollection(
                            ss.SiteId, Forms.List("CurrentPermissionsAll")).InTenant())
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "SearchPermissionElements":
                    case "OpenPermissionsDialog":
                    case "AddPermissions":
                    case "DeletePermissions":
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
            if (ss.Title != Forms.Data("DeleteSiteTitle") || !Authenticate())
            {
                return Error.Types.IncorrectSiteDeleting;
            }
            return ss.CanManageSite()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        private static bool Authenticate()
        {
            return Authentications.Windows() || Authentications.Try(
                Forms.Data("Users_LoginId"), Forms.Data("Users_Password").Sha512Cng());
        }

        public static Error.Types OnRestoring()
        {
            return Permissions.CanManageTenant()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnExporting(SiteSettings ss)
        {
            return ss.CanExport()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnShowingMenu(SiteModel siteModel)
        {
            return
                siteModel.SiteSettings.HasPermission() &&
                siteModel.AccessStatus != Databases.AccessStatuses.NotFound
                    ? Error.Types.None
                    : Error.Types.NotFound;
        }

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

        public static Error.Types OnSorting(SiteSettings ss)
        {
            if (ss.SiteId != 0 && !ss.CanManageSite())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnLinking(
            long sourceInheritSiteId, long destinationInheritSiteId)
        {
            if (!Permissions.Can(sourceInheritSiteId, Permissions.Types.ManageSite))
            {
                return Error.Types.HasNotPermission;
            }
            if (!Permissions.Can(destinationInheritSiteId, Permissions.Types.Read))
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnSetSiteSettings(SiteSettings ss, out string data)
        {
            data = null;
            if (!ss.CanManageSite())
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

        public static Error.Types InheritPermission(SiteSettings ss)
        {
            if (!ss.CanManagePermission())
            {
                return Error.Types.HasNotPermission;
            }
            var inheritPermission = Forms.Long("InheritPermission");
            if (ss.SiteId != inheritPermission)
            {
                if (!PermissionUtilities.InheritTargetsDataRows(ss.SiteId).Any(o =>
                    o["SiteId"].ToLong() == Forms.Long("InheritPermission")))
                {
                    return Error.Types.CanNotInherit;
                }
                if (!Permissions.CanRead(inheritPermission))
                {
                    return Error.Types.HasNotPermission;
                }
                if (PermissionUtilities.HasInheritedSites(ss.SiteId))
                {
                    return Error.Types.CanNotChangeInheritance;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types SetReminder(out string data)
        {
            data = null;
            var badFrom = MailAddressValidators.BadMailAddress(
                Forms.Data("ReminderFrom"), out data);
            if (badFrom != Error.Types.None) return badFrom;
            var badTo = MailAddressValidators.BadMailAddress(
                Forms.Data("ReminderTo"), out data);
            if (badTo != Error.Types.None) return badTo;
            var externalTo = MailAddressValidators.ExternalMailAddress(
                Forms.Data("ReminderTo"), out data);
            return Error.Types.None;
        }
    }
}
