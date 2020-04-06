using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class SiteValidators
    {
        public static ErrorData OnEntry(Context context, SiteSettings ss)
        {
            return context.HasPermission(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnReading(Context context, SiteSettings ss)
        {
            return context.CanManageSite(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnEditing(Context context, SiteSettings ss, SiteModel siteModel)
        {
            return
                context.CanManageSite(ss: ss) &&
                siteModel.AccessStatus != Databases.AccessStatuses.NotFound
                    ? new ErrorData(type: Error.Types.None)
                    : siteModel.MethodType == BaseModel.MethodTypes.New
                        ? new ErrorData(type: Error.Types.HasNotPermission)
                        : new ErrorData(type: Error.Types.NotFound);
        }

        public static ErrorData OnCreating(Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (!context.CanManageSite(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            ss.SetColumnAccessControls(context: context, mine: siteModel.Mine(context: context));
            foreach(var controlId in context.Forms.Keys)
            {
                switch (controlId)
                {
                    case "Sites_Title":
                        if (!ss.GetColumn(context: context, columnName: "Title").CanCreate)
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Sites_Body":
                        if (!ss.GetColumn(context: context, columnName: "Body").CanCreate)
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Sites_ReferenceType":
                        if (!ss.GetColumn(context: context, columnName: "ReferenceType").CanCreate)
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Sites_InheritPermission":
                        if (!ss.GetColumn(context: context, columnName: "InheritPermission").CanCreate)
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn(context: context, columnName: "Comments").CanCreate)
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                }
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnUpdating(Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (!context.CanManageSite(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            ss.SetColumnAccessControls(context: context, mine: siteModel.Mine(context: context));
            foreach(var controlId in context.Forms.Keys)
            {
                switch (controlId)
                {
                    case "Sites_Title":
                        if (siteModel.Title_Updated(context: context) &&
                            !ss.GetColumn(context: context, columnName: "Title").CanUpdate)
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Sites_Body":
                        if (siteModel.Body_Updated(context: context) &&
                            !ss.GetColumn(context: context, columnName: "Body").CanUpdate)
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Sites_ReferenceType":
                        if (siteModel.ReferenceType_Updated(context: context) &&
                            !ss.GetColumn(context: context, columnName: "ReferenceType").CanUpdate)
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Sites_InheritPermission":
                        if (siteModel.InheritPermission_Updated(context: context) &&
                            !ss.GetColumn(context: context, columnName: "InheritPermission").CanUpdate)
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "InheritPermission":
                        var errorData = InheritPermission(context: context, ss: ss);
                        if (errorData.Type != Error.Types.None) return errorData;
                        break;
                    case "CurrentPermissionsAll":
                        if (!context.CanManagePermission(ss: ss))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        if (!new PermissionCollection(
                            context: context,
                            referenceId: ss.SiteId,
                            permissions: context.Forms.List("CurrentPermissionsAll"))
                                .InTenant(context: context))
                        {
                            return new ErrorData(type: Error.Types.InvalidRequest);
                        }
                        break;
                    case "SearchPermissionElements":
                    case "OpenPermissionsDialog":
                    case "AddPermissions":
                    case "DeletePermissions":
                        if (!context.CanManagePermission(ss: ss))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn(context: context, columnName: "Comments").CanUpdate)
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                }
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnDeleting(Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (ss.Title != context.Forms.Data("DeleteSiteTitle")
                || !Authenticate(context: context))
            {
                return new ErrorData(type: Error.Types.IncorrectSiteDeleting);
            }
            return context.CanManageSite(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        private static bool Authenticate(Context context)
        {
            return Authentications.SSO(context: context) 
                || Authentications.Try(
                    context: context,
                    loginId: context.Forms.Data("Users_LoginId"),
                    password: context.Forms.Data("Users_Password").Sha512Cng());
        }

        public static ErrorData OnRestoring(Context context)
        {
            return Permissions.CanManageTenant(context: context)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnExporting(Context context, SiteSettings ss)
        {
            return context.CanExport(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnShowingMenu(Context context, SiteModel siteModel)
        {
            return context.HasPermission(ss: siteModel.SiteSettings)
                && siteModel.AccessStatus != Databases.AccessStatuses.NotFound
                    ? new ErrorData(type: Error.Types.None)
                    : new ErrorData(type: Error.Types.NotFound);
        }

        public static ErrorData OnMoving(
            Context context,
            long currentId,
            long destinationId,
            SiteSettings current,
            SiteSettings source,
            SiteSettings destination)
        {
            if (currentId != 0 && !context.CanManageSite(ss: current))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            if (!context.CanManageSite(ss: source))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            if (destinationId != 0 && !context.CanManageSite(ss: destination))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnSorting(Context context, SiteSettings ss)
        {
            if (ss.SiteId != 0 && !context.CanManageSite(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnLinking(
            Context context, long sourceInheritSiteId, long destinationInheritSiteId)
        {
            if (!Permissions.Can(
                context: context,
                siteId: sourceInheritSiteId,
                type: Permissions.Types.ManageSite))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            if (!Permissions.Can(
                context: context,
                siteId: destinationInheritSiteId,
                type: Permissions.Types.Read))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnSetSiteSettings(
            Context context, SiteSettings ss, out string data)
        {
            data = null;
            if (!context.CanManageSite(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            foreach (var key in context.Forms.Keys)
            {
                switch (key)
                {
                    case "Format":
                        try
                        {
                            0.ToString(
                                format: context.Forms.Get(key),
                                provider: context.CultureInfo());
                        }
                        catch (System.Exception)
                        {
                            data = context.Forms.Get(key);
                            return new ErrorData(type: Error.Types.BadFormat);
                        }
                        break;
                }
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData InheritPermission(Context context, SiteSettings ss)
        {
            if (!context.CanManagePermission(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            var inheritPermission = context.Forms.Long("InheritPermission");
            if (ss.SiteId != inheritPermission)
            {
                if (!PermissionUtilities.InheritTargetsDataRows(
                    context: context,
                    ss: ss).Any(o =>
                        o.Long("SiteId") == context.Forms.Long("InheritPermission")))
                {
                    return new ErrorData(type: Error.Types.CanNotInherit);
                }
                if (!Permissions.CanRead(
                    context: context,
                    siteId: inheritPermission))
                {
                    return new ErrorData(type: Error.Types.HasNotPermission);
                }
                if (PermissionUtilities.HasInheritedSites(context: context, siteId: ss.SiteId))
                {
                    return new ErrorData(type: Error.Types.CanNotChangeInheritance);
                }
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData SetReminder(Context context)
        {
            var badFrom = MailAddressValidators.BadMailAddress(
                context: context,
                addresses: context.Forms.Data("ReminderFrom"));
            if (badFrom.Type != Error.Types.None) return badFrom;
            var badTo = MailAddressValidators.BadMailAddress(
                context: context,
                addresses: context.Forms.Data("ReminderTo"));
            if (badTo.Type != Error.Types.None) return badTo;
            var externalTo = MailAddressValidators.ExternalMailAddress(
                context: context,
                addresses: context.Forms.Data("ReminderTo"));
            if (externalTo.Type != Error.Types.None) return externalTo;
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnReading(Context context, SiteSettings ss, SiteModel siteModel)
        {
            return context.CanRead(ss, true)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnLockTable(Context context, SiteSettings ss)
        {
            return context.CanManageSite(ss: ss)
                && !ss.Locked()
                    ? new ErrorData(type: Error.Types.None)
                    : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnUnlockTable(Context context, SiteSettings ss)
        {
            return ss.Locked()
                && (ss.LockedUser.Id == context.UserId || context.HasPrivilege)
                    ? new ErrorData(type: Error.Types.None)
                    : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnForceUnlockTable(Context context, SiteSettings ss)
        {
            return ss.Locked()
                && context.HasPrivilege
                    ? new ErrorData(type: Error.Types.None)
                    : new ErrorData(type: Error.Types.HasNotPermission);
        }
    }
}
