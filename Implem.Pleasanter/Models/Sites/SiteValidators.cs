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
        public static Error.Types OnEntry(Context context, SiteSettings ss)
        {
            return context.HasPermission(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnReading(Context context, SiteSettings ss)
        {
            return context.CanManageSite(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnEditing(Context context, SiteSettings ss, SiteModel siteModel)
        {
            return
                context.CanManageSite(ss: ss) &&
                siteModel.AccessStatus != Databases.AccessStatuses.NotFound
                    ? Error.Types.None
                    : siteModel.MethodType == BaseModel.MethodTypes.New
                        ? Error.Types.HasNotPermission
                        : Error.Types.NotFound;
        }

        public static Error.Types OnCreating(Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (!context.CanManageSite(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(context: context, mine: siteModel.Mine(context: context));
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Sites_Title":
                        if (!ss.GetColumn(context: context, columnName: "Title").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Sites_Body":
                        if (!ss.GetColumn(context: context, columnName: "Body").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Sites_ReferenceType":
                        if (!ss.GetColumn(context: context, columnName: "ReferenceType").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Sites_InheritPermission":
                        if (!ss.GetColumn(context: context, columnName: "InheritPermission").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn(context: context, columnName: "Comments").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (!context.CanManageSite(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(context: context, mine: siteModel.Mine(context: context));
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Sites_Title":
                        if (siteModel.Title_Updated(context: context) &&
                            !ss.GetColumn(context: context, columnName: "Title").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Sites_Body":
                        if (siteModel.Body_Updated(context: context) &&
                            !ss.GetColumn(context: context, columnName: "Body").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Sites_ReferenceType":
                        if (siteModel.ReferenceType_Updated(context: context) &&
                            !ss.GetColumn(context: context, columnName: "ReferenceType").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Sites_InheritPermission":
                        if (siteModel.InheritPermission_Updated(context: context) &&
                            !ss.GetColumn(context: context, columnName: "InheritPermission").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "InheritPermission":
                        var type = InheritPermission(context: context, ss: ss);
                        if (type != Error.Types.None) return type;
                        break;
                    case "CurrentPermissionsAll":
                        if (!context.CanManagePermission(ss: ss))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        if (!new PermissionCollection(
                            context: context,
                            referenceId: ss.SiteId,
                            permissions: Forms.List("CurrentPermissionsAll"))
                                .InTenant(context: context))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "SearchPermissionElements":
                    case "OpenPermissionsDialog":
                    case "AddPermissions":
                    case "DeletePermissions":
                        if (!context.CanManagePermission(ss: ss))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn(context: context, columnName: "Comments").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (ss.Title != Forms.Data("DeleteSiteTitle") || !Authenticate(context: context))
            {
                return Error.Types.IncorrectSiteDeleting;
            }
            return context.CanManageSite(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        private static bool Authenticate(Context context)
        {
            return Authentications.Windows() || Authentications.Try(
                context: context,
                loginId: Forms.Data("Users_LoginId"),
                password: Forms.Data("Users_Password").Sha512Cng());
        }

        public static Error.Types OnRestoring(Context context)
        {
            return Permissions.CanManageTenant(context: context)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnExporting(Context context, SiteSettings ss)
        {
            return context.CanExport(ss: ss)
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnShowingMenu(Context context, SiteModel siteModel)
        {
            return context.HasPermission(ss: siteModel.SiteSettings)
                && siteModel.AccessStatus != Databases.AccessStatuses.NotFound
                    ? Error.Types.None
                    : Error.Types.NotFound;
        }

        public static Error.Types OnMoving(
            Context context,
            long currentId,
            long destinationId,
            SiteSettings current,
            SiteSettings source,
            SiteSettings destination)
        {
            if (currentId != 0 && !context.CanManageSite(ss: current))
            {
                return Error.Types.HasNotPermission;
            }
            if (!context.CanManageSite(ss: source))
            {
                return Error.Types.HasNotPermission;
            }
            if (destinationId != 0 && !context.CanManageSite(ss: destination))
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnSorting(Context context, SiteSettings ss)
        {
            if (ss.SiteId != 0 && !context.CanManageSite(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnLinking(
            Context context, long sourceInheritSiteId, long destinationInheritSiteId)
        {
            if (!Permissions.Can(
                context: context,
                siteId: sourceInheritSiteId,
                type: Permissions.Types.ManageSite))
            {
                return Error.Types.HasNotPermission;
            }
            if (!Permissions.Can(
                context: context,
                siteId: destinationInheritSiteId,
                type: Permissions.Types.Read))
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnSetSiteSettings(
            Context context, SiteSettings ss, out string data)
        {
            data = null;
            if (!context.CanManageSite(ss: ss))
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

        public static Error.Types InheritPermission(Context context, SiteSettings ss)
        {
            if (!context.CanManagePermission(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            var inheritPermission = Forms.Long("InheritPermission");
            if (ss.SiteId != inheritPermission)
            {
                if (!PermissionUtilities.InheritTargetsDataRows(
                    context: context,
                    ss: ss).Any(o =>
                        o.Long("SiteId") == Forms.Long("InheritPermission")))
                {
                    return Error.Types.CanNotInherit;
                }
                if (!Permissions.CanRead(
                    context: context,
                    siteId: inheritPermission))
                {
                    return Error.Types.HasNotPermission;
                }
                if (PermissionUtilities.HasInheritedSites(context: context, siteId: ss.SiteId))
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnReading(Context context, SiteSettings ss, SiteModel siteModel)
        {
            return context.CanRead(ss, true)
                    ? Error.Types.None
                    : Error.Types.HasNotPermission;
        }
    }
}
