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
        public static ErrorData OnEntry(Context context, SiteSettings ss, bool api = false)
        {
            if (api)
            {
                if ((!Parameters.Api.Enabled
                    || context.ContractSettings.Api == false
                    || context.UserSettings?.AllowApi(context: context) == false))
                {
                    return new ErrorData(type: Error.Types.InvalidRequest);
                }
                if (context.InvalidJsonData)
                {
                    return new ErrorData(type: Error.Types.InvalidJsonData);
                }
                if (!ss.IsSite(context: context))
                {
                    return new ErrorData(type: Error.Types.NotFound);
                }
            }
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
            foreach(var controlId in context.Forms.Keys)
            {
                switch (controlId)
                {
                    case "Sites_Title":
                        if (!ss.GetColumn(
                            context: context,
                            columnName: "Title")
                                .CanCreate(
                                    context: context,
                                    ss: ss,
                                    mine: null))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Sites_Body":
                        if (!ss.GetColumn(
                            context: context,
                            columnName: "Body")
                                .CanCreate(
                                    context: context,
                                    ss: ss,
                                    mine: null))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Sites_ReferenceType":
                        if (!ss.GetColumn(
                            context: context,
                            columnName: "ReferenceType")
                                .CanCreate(
                                    context: context,
                                    ss: ss,
                                    mine: null))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Sites_InheritPermission":
                        if (!ss.GetColumn(
                            context: context,
                            columnName: "InheritPermission")
                                .CanCreate(
                                    context: context,
                                    ss: ss,
                                    mine: null))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn(
                            context: context,
                            columnName: "Comments")
                                .CanCreate(
                                    context: context,
                                    ss: ss,
                                    mine: null))
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
            if (siteModel.InheritPermission_Updated(context: context)
                && !context.CanManagePermission(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            if (siteModel.RecordPermissions != null
                && !context.CanManagePermission(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            foreach (var controlId in context.Forms.Keys)
            {
                switch (controlId)
                {
                    case "Sites_Title":
                        if (siteModel.Title_Updated(context: context) &&
                            !ss.GetColumn(
                                context: context,
                                columnName: "Title")
                                    .CanUpdate(
                                        context: context,
                                        ss: ss,
                                        mine: null))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Sites_Body":
                        if (siteModel.Body_Updated(context: context) &&
                            !ss.GetColumn(
                                context: context,
                                columnName: "Body")
                                    .CanUpdate(
                                        context: context,
                                        ss: ss,
                                        mine: null))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Sites_ReferenceType":
                        if (siteModel.ReferenceType_Updated(context: context) &&
                            !ss.GetColumn(
                                context: context,
                                columnName: "ReferenceType")
                                    .CanUpdate(
                                        context: context,
                                        ss: ss,
                                        mine: null))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Sites_InheritPermission":
                        if (siteModel.InheritPermission_Updated(context: context) &&
                            !ss.GetColumn(
                                context: context,
                                columnName: "InheritPermission")
                                    .CanUpdate(
                                        context: context,
                                        ss: ss,
                                        mine: null))
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
                        if (!ss.GetColumn(
                            context: context,
                            columnName: "Comments")
                                .CanUpdate(
                                    context: context,
                                    ss: ss,
                                    mine: null))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "AddressBook":
                    case "MailToDefault":
                    case "MailCcDefault":
                    case "MailBccDefault":
                        var address = context.Forms.Get(controlId)
                            .Split(';')
                            .Where(o => !o.IsNullOrEmpty())
                            .Select(o => o.Trim())
                            .Join();
                        var badTo = MailAddressValidators.BadMailAddress(
                            addresses: address);
                        if (badTo.Type != Error.Types.None) return badTo;
                        var externalTo = MailAddressValidators.ExternalMailAddress(
                            addresses: address);
                        if (externalTo.Type != Error.Types.None) return externalTo;
                        break;
                }
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnDeleting(
            Context context,
            SiteSettings ss,
            SiteModel siteModel,
            bool api = false,
            bool serverScript = false)
        {
            if (api)
            {
                var apiErrorData = Validators.ValidateApi(
                    context: context,
                    serverScript: serverScript);
                if (apiErrorData.Type != Error.Types.None)
                {
                    return apiErrorData;
                }
            }
            else if (ss.Title != context.Forms.Data("DeleteSiteTitle")
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
            return Authentications.DisableDeletingSiteAuthentication(context: context)
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

        public static ErrorData OnShowingMenu(Context context, SiteSettings ss, SiteModel siteModel)
        {
            if (ss.GetNoDisplayIfReadOnly(context: context))
            {
                return new ErrorData(type: Error.Types.NotFound);
            }
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
            if (destinationId == 0
                && context.UserSettings?.AllowCreationAtTopSite(context: context) != true)
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            if (currentId == 0
                && context.UserSettings?.AllowMovingFromTopSite(context: context) != true)
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
                    ss: ss).DataRows.Any(o =>
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

        public static ErrorData SetReminder(Context context, SiteSettings ss)
        {
            if ((Reminder.ReminderTypes)context.Forms.Int("ReminderType") == Reminder.ReminderTypes.Mail)
            {
                var badFrom = MailAddressValidators.BadMailAddress(
                    addresses: context.Forms.Data("ReminderFrom"));
                if (badFrom.Type != Error.Types.None) return badFrom;
                var to = ss.LabelTextToColumnName(context.Forms.Data("ReminderTo"));
                return SetMailTo(
                    ss: ss,
                    to: to);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData SetNotification(Context context, SiteSettings ss)
        {
            if ((Notification.Types)context.Forms.Int("NotificationType") == Notification.Types.Mail)
            {
                var to = ss.LabelTextToColumnName(context.Forms.Data("NotificationAddress"));
                var cc = ss.LabelTextToColumnName(context.Forms.Data("NotificationCcAddress"));
                var bcc = ss.LabelTextToColumnName(context.Forms.Data("NotificationBccAddress"));
                return SetMailTo(
                    ss: ss,
                    to: to,
                    cc: cc,
                    bcc: bcc,
                    notification: true);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData SetProcessNotification(Context context, SiteSettings ss)
        {
            if ((Notification.Types)context.Forms.Int("ProcessNotificationType") == Notification.Types.Mail)
            {
                var to = ss.LabelTextToColumnName(context.Forms.Data("ProcessNotificationAddress"));
                var cc = ss.LabelTextToColumnName(context.Forms.Data("ProcessNotificationCcAddress"));
                var bcc = ss.LabelTextToColumnName(context.Forms.Data("ProcessNotificationBccAddress"));
                return SetMailTo(
                    ss: ss,
                    to: to,
                    cc: cc,
                    bcc: bcc,
                    notification: true);
            }
            return new ErrorData(type: Error.Types.None);
        }

        private static ErrorData SetMailTo(SiteSettings ss, string to, bool notification = false)
        {
            ss.IncludedColumns(value: to).ForEach(column =>
                to = to.Replace($"[{column.ColumnName}]", string.Empty));
            foreach (System.Text.RegularExpressions.Match match in to.RegexMatches(@"(\[Dept[0-9]+\])"))
            {
                to = to.Replace(match.Value, string.Empty);
            }
            foreach (System.Text.RegularExpressions.Match match in to.RegexMatches(@"(\[Group[0-9]+\])"))
            {
                to = to.Replace(match.Value, string.Empty);
            }
            foreach (System.Text.RegularExpressions.Match match in to.RegexMatches(@"(\[User[0-9]+\])"))
            {
                to = to.Replace(match.Value, string.Empty);
            }
            if (notification)
            {
                //通知機能のみ[RelatedUsers]の指定が可能なため、[RelatedUsers]の指定をチェック。
                foreach (System.Text.RegularExpressions.Match match in to.RegexMatches(@"(\[RelatedUsers\])"))
                {
                    to = to.Replace(match.Value, string.Empty);
                }
            }
            to = to
                .Split(',')
                .Where(o => !o.IsNullOrEmpty())
                .Select(o => o.Trim())
                .Join();
            var badTo = MailAddressValidators.BadMailAddress(
                addresses: to);
            if (badTo.Type != Error.Types.None) return badTo;
            var externalTo = MailAddressValidators.ExternalMailAddress(
                addresses: to);
            if (externalTo.Type != Error.Types.None) return externalTo;
            return new ErrorData(type: Error.Types.None);
        }

        private static ErrorData SetMailTo(SiteSettings ss, string to, string cc, string bcc, bool notification = false)
        {
            ss.IncludedColumns(value: to).ForEach(column =>
                to = to.Replace($"[{column.ColumnName}]", string.Empty));
            foreach (System.Text.RegularExpressions.Match match in to.RegexMatches(@"(\[Dept[0-9]+\])"))
            {
                to = to.Replace(match.Value, string.Empty);
            }
            foreach (System.Text.RegularExpressions.Match match in to.RegexMatches(@"(\[Group[0-9]+\])"))
            {
                to = to.Replace(match.Value, string.Empty);
            }
            foreach (System.Text.RegularExpressions.Match match in to.RegexMatches(@"(\[User[0-9]+\])"))
            {
                to = to.Replace(match.Value, string.Empty);
            }

            ss.IncludedColumns(value: cc).ForEach(column =>
                cc = cc.Replace($"[{column.ColumnName}]", string.Empty));
            foreach (System.Text.RegularExpressions.Match match in cc.RegexMatches(@"(\[Dept[0-9]+\])"))
            {
                cc = cc.Replace(match.Value, string.Empty);
            }
            foreach (System.Text.RegularExpressions.Match match in cc.RegexMatches(@"(\[Group[0-9]+\])"))
            {
                cc = cc.Replace(match.Value, string.Empty);
            }
            foreach (System.Text.RegularExpressions.Match match in cc.RegexMatches(@"(\[User[0-9]+\])"))
            {
                cc = cc.Replace(match.Value, string.Empty);
            }

            ss.IncludedColumns(value: bcc).ForEach(column =>
                bcc = bcc.Replace($"[{column.ColumnName}]", string.Empty));
            foreach (System.Text.RegularExpressions.Match match in bcc.RegexMatches(@"(\[Dept[0-9]+\])"))
            {
                bcc = bcc.Replace(match.Value, string.Empty);
            }
            foreach (System.Text.RegularExpressions.Match match in bcc.RegexMatches(@"(\[Group[0-9]+\])"))
            {
                bcc = bcc.Replace(match.Value, string.Empty);
            }
            foreach (System.Text.RegularExpressions.Match match in bcc.RegexMatches(@"(\[User[0-9]+\])"))
            {
                bcc = bcc.Replace(match.Value, string.Empty);
            }

            if (notification)
            {
                //通知機能のみ[RelatedUsers]の指定が可能なため、[RelatedUsers]の指定をチェック。
                foreach (System.Text.RegularExpressions.Match match in to.RegexMatches(@"(\[RelatedUsers\])"))
                {
                    to = to.Replace(match.Value, string.Empty);
                }
                foreach (System.Text.RegularExpressions.Match match in cc.RegexMatches(@"(\[RelatedUsers\])"))
                {
                    cc = cc.Replace(match.Value, string.Empty);
                }
                foreach (System.Text.RegularExpressions.Match match in bcc.RegexMatches(@"(\[RelatedUsers\])"))
                {
                    bcc = bcc.Replace(match.Value, string.Empty);
                }
            }
            to = to
                .Split(',')
                .Where(o => !o.IsNullOrEmpty())
                .Select(o => o.Trim())
                .Join();
            var badTo = MailAddressValidators.BadMailAddress(
                addresses: to);
            if (badTo.Type != Error.Types.None) return badTo;
            var externalTo = MailAddressValidators.ExternalMailAddress(
                addresses: to);
            if (externalTo.Type != Error.Types.None) return externalTo;

            cc = cc
                .Split(',')
                .Where(o => !o.IsNullOrEmpty())
                .Select(o => o.Trim())
                .Join();
            var badCc = MailAddressValidators.BadMailAddress(
                addresses: cc);
            if (badCc.Type != Error.Types.None) return badCc;
            var externalCc = MailAddressValidators.ExternalMailAddress(
                addresses: cc);
            if (externalCc.Type != Error.Types.None) return externalCc;

            bcc = bcc
                .Split(',')
                .Where(o => !o.IsNullOrEmpty())
                .Select(o => o.Trim())
                .Join();
            var badBcc = MailAddressValidators.BadMailAddress(
                addresses: bcc);
            if (badBcc.Type != Error.Types.None) return badBcc;
            var externalBcc = MailAddressValidators.ExternalMailAddress(
                addresses: bcc);
            if (externalBcc.Type != Error.Types.None) return externalBcc;

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
                && (ss.LockedTableUser.Id == context.UserId || context.HasPrivilege)
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
