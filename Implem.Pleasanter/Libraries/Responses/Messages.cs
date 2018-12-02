using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class Messages
    {
        private static Message Get(string text, string css)
        {
            var hb = new HtmlBuilder();
            return new Message(text, css);
        }

        private static ResponseCollection ResponseMessage(Message message)
        {
            return new ResponseCollection().Message(message);
        }

        public static ResponseCollection ResponseMessage(this PasswordPolicy policy, Context context)
        {
            return new ResponseCollection().Message(policy.Languages?.Any() == true
                ? new Message(
                    text: policy.Display(context: context),
                    css: "alert-error")
                : PasswordPolicyViolation(context: context));
        }

        public static Message AlreadyAdded(Context context, params string[] data)
        {
            return Get(
                text: Displays.AlreadyAdded(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message AlreadyLinked(Context context, params string[] data)
        {
            return Get(
                text: Displays.AlreadyLinked(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ApiKeyCreated(Context context, params string[] data)
        {
            return Get(
                text: Displays.ApiKeyCreated(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message ApiKeyDeleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.ApiKeyDeleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message ApplicationError(Context context, params string[] data)
        {
            return Get(
                text: Displays.ApplicationError(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message Authentication(Context context, params string[] data)
        {
            return Get(
                text: Displays.Authentication(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message BadFormat(Context context, params string[] data)
        {
            return Get(
                text: Displays.BadFormat(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message BadMailAddress(Context context, params string[] data)
        {
            return Get(
                text: Displays.BadMailAddress(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message BadRequest(Context context, params string[] data)
        {
            return Get(
                text: Displays.BadRequest(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message BulkDeleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.BulkDeleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message BulkMoved(Context context, params string[] data)
        {
            return Get(
                text: Displays.BulkMoved(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message BulkRestored(Context context, params string[] data)
        {
            return Get(
                text: Displays.BulkRestored(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CanNotChangeInheritance(Context context, params string[] data)
        {
            return Get(
                text: Displays.CanNotChangeInheritance(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CanNotDisabled(Context context, params string[] data)
        {
            return Get(
                text: Displays.CanNotDisabled(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CanNotInherit(Context context, params string[] data)
        {
            return Get(
                text: Displays.CanNotInherit(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CanNotLink(Context context, params string[] data)
        {
            return Get(
                text: Displays.CanNotLink(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CanNotPerformed(Context context, params string[] data)
        {
            return Get(
                text: Displays.CanNotPerformed(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CanNotUpdate(Context context, params string[] data)
        {
            return Get(
                text: Displays.CanNotUpdate(
                    context: context,
                    data: data),
                css: "alert-info");
        }

        public static Message CantSetAtTopOfSite(Context context, params string[] data)
        {
            return Get(
                text: Displays.CantSetAtTopOfSite(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ChangingPasswordComplete(Context context, params string[] data)
        {
            return Get(
                text: Displays.ChangingPasswordComplete(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerBackupCompleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.CodeDefinerBackupCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerCompleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.CodeDefinerCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerCssCompleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.CodeDefinerCssCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerDefCompleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.CodeDefinerDefCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerInsertTestDataCompleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.CodeDefinerInsertTestDataCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerMvcCompleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.CodeDefinerMvcCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerRdsCompleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.CodeDefinerRdsCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CommentDeleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.CommentDeleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message Copied(Context context, params string[] data)
        {
            return Get(
                text: Displays.Copied(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message Created(Context context, params string[] data)
        {
            return Get(
                text: Displays.Created(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message DefinitionNotFound(Context context, params string[] data)
        {
            return Get(
                text: Displays.DefinitionNotFound(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message DeleteConflicts(Context context, params string[] data)
        {
            return Get(
                text: Displays.DeleteConflicts(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message Deleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.Deleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message DeletedImage(Context context, params string[] data)
        {
            return Get(
                text: Displays.DeletedImage(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message Duplicated(Context context, params string[] data)
        {
            return Get(
                text: Displays.Duplicated(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message Expired(Context context, params string[] data)
        {
            return Get(
                text: Displays.Expired(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ExternalMailAddress(Context context, params string[] data)
        {
            return Get(
                text: Displays.ExternalMailAddress(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message FailedReadFile(Context context, params string[] data)
        {
            return Get(
                text: Displays.FailedReadFile(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message FileDeleteCompleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.FileDeleteCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message FileDragDrop(Context context, params string[] data)
        {
            return Get(
                text: Displays.FileDragDrop(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message FileNotFound(Context context, params string[] data)
        {
            return Get(
                text: Displays.FileNotFound(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message FileUpdateCompleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.FileUpdateCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message HasBeenDeleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.HasBeenDeleted(
                    context: context,
                    data: data),
                css: "alert-info");
        }

        public static Message HasBeenMoved(Context context, params string[] data)
        {
            return Get(
                text: Displays.HasBeenMoved(
                    context: context,
                    data: data),
                css: "alert-info");
        }

        public static Message HasNotPermission(Context context, params string[] data)
        {
            return Get(
                text: Displays.HasNotPermission(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message HistoryDeleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.HistoryDeleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message Imported(Context context, params string[] data)
        {
            return Get(
                text: Displays.Imported(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message ImportMax(Context context, params string[] data)
        {
            return Get(
                text: Displays.ImportMax(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InCompression(Context context, params string[] data)
        {
            return Get(
                text: Displays.InCompression(
                    context: context,
                    data: data),
                css: "alert-info");
        }

        public static Message InCopying(Context context, params string[] data)
        {
            return Get(
                text: Displays.InCopying(
                    context: context,
                    data: data),
                css: "alert-info");
        }

        public static Message IncorrectCurrentPassword(Context context, params string[] data)
        {
            return Get(
                text: Displays.IncorrectCurrentPassword(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message IncorrectFileFormat(Context context, params string[] data)
        {
            return Get(
                text: Displays.IncorrectFileFormat(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message IncorrectSiteDeleting(Context context, params string[] data)
        {
            return Get(
                text: Displays.IncorrectSiteDeleting(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InputMailAddress(Context context, params string[] data)
        {
            return Get(
                text: Displays.InputMailAddress(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InternalServerError(Context context, params string[] data)
        {
            return Get(
                text: Displays.InternalServerError(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InvalidCsvData(Context context, params string[] data)
        {
            return Get(
                text: Displays.InvalidCsvData(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InvalidFormula(Context context, params string[] data)
        {
            return Get(
                text: Displays.InvalidFormula(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InvalidRequest(Context context, params string[] data)
        {
            return Get(
                text: Displays.InvalidRequest(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ItemsLimit(Context context, params string[] data)
        {
            return Get(
                text: Displays.ItemsLimit(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message LinkCreated(Context context, params string[] data)
        {
            return Get(
                text: Displays.LinkCreated(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message LoginIdAlreadyUse(Context context, params string[] data)
        {
            return Get(
                text: Displays.LoginIdAlreadyUse(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message LoginIn(Context context, params string[] data)
        {
            return Get(
                text: Displays.LoginIn(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message MailAddressHasNotSet(Context context, params string[] data)
        {
            return Get(
                text: Displays.MailAddressHasNotSet(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message MailTransmissionCompletion(Context context, params string[] data)
        {
            return Get(
                text: Displays.MailTransmissionCompletion(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message Moved(Context context, params string[] data)
        {
            return Get(
                text: Displays.Moved(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message NoLinks(Context context, params string[] data)
        {
            return Get(
                text: Displays.NoLinks(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message NotFound(Context context, params string[] data)
        {
            return Get(
                text: Displays.NotFound(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message NotRequiredColumn(Context context, params string[] data)
        {
            return Get(
                text: Displays.NotRequiredColumn(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message OverLimitQuantity(Context context, params string[] data)
        {
            return Get(
                text: Displays.OverLimitQuantity(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message OverLimitSize(Context context, params string[] data)
        {
            return Get(
                text: Displays.OverLimitSize(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message OverTenantStorageSize(Context context, params string[] data)
        {
            return Get(
                text: Displays.OverTenantStorageSize(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message OverTotalLimitSize(Context context, params string[] data)
        {
            return Get(
                text: Displays.OverTotalLimitSize(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ParameterSyntaxError(Context context, params string[] data)
        {
            return Get(
                text: Displays.ParameterSyntaxError(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message PasswordNotChanged(Context context, params string[] data)
        {
            return Get(
                text: Displays.PasswordNotChanged(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message PasswordPolicyViolation(Context context, params string[] data)
        {
            return Get(
                text: Displays.PasswordPolicyViolation(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message PasswordResetCompleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.PasswordResetCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message PermissionNotSelfChange(Context context, params string[] data)
        {
            return Get(
                text: Displays.PermissionNotSelfChange(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message PhysicalDeleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.PhysicalDeleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message ReadOnlyBecausePreviousVer(Context context, params string[] data)
        {
            return Get(
                text: Displays.ReadOnlyBecausePreviousVer(
                    context: context,
                    data: data),
                css: "alert-info");
        }

        public static Message RebuildingCompleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.RebuildingCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message RequireMailAddresses(Context context, params string[] data)
        {
            return Get(
                text: Displays.RequireMailAddresses(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message RequireTo(Context context, params string[] data)
        {
            return Get(
                text: Displays.RequireTo(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message RestoredFromHistory(Context context, params string[] data)
        {
            return Get(
                text: Displays.RestoredFromHistory(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message Restricted(Context context, params string[] data)
        {
            return Get(
                text: Displays.Restricted(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message SelectFile(Context context, params string[] data)
        {
            return Get(
                text: Displays.SelectFile(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message SelectOne(Context context, params string[] data)
        {
            return Get(
                text: Displays.SelectOne(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message SelectTargets(Context context, params string[] data)
        {
            return Get(
                text: Displays.SelectTargets(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message SentAcceptanceMail (Context context, params string[] data)
        {
            return Get(
                text: Displays.SentAcceptanceMail (
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message Separated(Context context, params string[] data)
        {
            return Get(
                text: Displays.Separated(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message SitesCreated(Context context, params string[] data)
        {
            return Get(
                text: Displays.SitesCreated(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message SitesLimit(Context context, params string[] data)
        {
            return Get(
                text: Displays.SitesLimit(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message SynchronizationCompleted(Context context, params string[] data)
        {
            return Get(
                text: Displays.SynchronizationCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message TooManyCases(Context context, params string[] data)
        {
            return Get(
                text: Displays.TooManyCases(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message TooManyColumnCases(Context context, params string[] data)
        {
            return Get(
                text: Displays.TooManyColumnCases(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message TooManyRowCases(Context context, params string[] data)
        {
            return Get(
                text: Displays.TooManyRowCases(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message Unauthorized(Context context, params string[] data)
        {
            return Get(
                text: Displays.Unauthorized(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message UpdateConflicts(Context context, params string[] data)
        {
            return Get(
                text: Displays.UpdateConflicts(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message Updated(Context context, params string[] data)
        {
            return Get(
                text: Displays.Updated(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message UserDisabled(Context context, params string[] data)
        {
            return Get(
                text: Displays.UserDisabled(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message UserLockout(Context context, params string[] data)
        {
            return Get(
                text: Displays.UserLockout(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message UsersLimit(Context context, params string[] data)
        {
            return Get(
                text: Displays.UsersLimit(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static ResponseCollection ResponseAlreadyAdded(Context context, params string[] data)
        {
            return ResponseMessage(AlreadyAdded(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseAlreadyLinked(Context context, params string[] data)
        {
            return ResponseMessage(AlreadyLinked(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseApiKeyCreated(Context context, params string[] data)
        {
            return ResponseMessage(ApiKeyCreated(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseApiKeyDeleted(Context context, params string[] data)
        {
            return ResponseMessage(ApiKeyDeleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseApplicationError(Context context, params string[] data)
        {
            return ResponseMessage(ApplicationError(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseAuthentication(Context context, params string[] data)
        {
            return ResponseMessage(Authentication(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseBadFormat(Context context, params string[] data)
        {
            return ResponseMessage(BadFormat(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseBadMailAddress(Context context, params string[] data)
        {
            return ResponseMessage(BadMailAddress(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseBadRequest(Context context, params string[] data)
        {
            return ResponseMessage(BadRequest(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseBulkDeleted(Context context, params string[] data)
        {
            return ResponseMessage(BulkDeleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseBulkMoved(Context context, params string[] data)
        {
            return ResponseMessage(BulkMoved(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseBulkRestored(Context context, params string[] data)
        {
            return ResponseMessage(BulkRestored(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCanNotChangeInheritance(Context context, params string[] data)
        {
            return ResponseMessage(CanNotChangeInheritance(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCanNotDisabled(Context context, params string[] data)
        {
            return ResponseMessage(CanNotDisabled(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCanNotInherit(Context context, params string[] data)
        {
            return ResponseMessage(CanNotInherit(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCanNotLink(Context context, params string[] data)
        {
            return ResponseMessage(CanNotLink(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCanNotPerformed(Context context, params string[] data)
        {
            return ResponseMessage(CanNotPerformed(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCanNotUpdate(Context context, params string[] data)
        {
            return ResponseMessage(CanNotUpdate(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCantSetAtTopOfSite(Context context, params string[] data)
        {
            return ResponseMessage(CantSetAtTopOfSite(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseChangingPasswordComplete(Context context, params string[] data)
        {
            return ResponseMessage(ChangingPasswordComplete(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCodeDefinerBackupCompleted(Context context, params string[] data)
        {
            return ResponseMessage(CodeDefinerBackupCompleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCodeDefinerCompleted(Context context, params string[] data)
        {
            return ResponseMessage(CodeDefinerCompleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCodeDefinerCssCompleted(Context context, params string[] data)
        {
            return ResponseMessage(CodeDefinerCssCompleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCodeDefinerDefCompleted(Context context, params string[] data)
        {
            return ResponseMessage(CodeDefinerDefCompleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCodeDefinerInsertTestDataCompleted(Context context, params string[] data)
        {
            return ResponseMessage(CodeDefinerInsertTestDataCompleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCodeDefinerMvcCompleted(Context context, params string[] data)
        {
            return ResponseMessage(CodeDefinerMvcCompleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCodeDefinerRdsCompleted(Context context, params string[] data)
        {
            return ResponseMessage(CodeDefinerRdsCompleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCommentDeleted(Context context, params string[] data)
        {
            return ResponseMessage(CommentDeleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCopied(Context context, params string[] data)
        {
            return ResponseMessage(Copied(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseCreated(Context context, params string[] data)
        {
            return ResponseMessage(Created(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseDefinitionNotFound(Context context, params string[] data)
        {
            return ResponseMessage(DefinitionNotFound(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseDeleteConflicts(Context context, params string[] data)
        {
            return ResponseMessage(DeleteConflicts(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseDeleted(Context context, params string[] data)
        {
            return ResponseMessage(Deleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseDeletedImage(Context context, params string[] data)
        {
            return ResponseMessage(DeletedImage(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseDuplicated(Context context, params string[] data)
        {
            return ResponseMessage(Duplicated(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseExpired(Context context, params string[] data)
        {
            return ResponseMessage(Expired(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseExternalMailAddress(Context context, params string[] data)
        {
            return ResponseMessage(ExternalMailAddress(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseFailedReadFile(Context context, params string[] data)
        {
            return ResponseMessage(FailedReadFile(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseFileDeleteCompleted(Context context, params string[] data)
        {
            return ResponseMessage(FileDeleteCompleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseFileDragDrop(Context context, params string[] data)
        {
            return ResponseMessage(FileDragDrop(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseFileNotFound(Context context, params string[] data)
        {
            return ResponseMessage(FileNotFound(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseFileUpdateCompleted(Context context, params string[] data)
        {
            return ResponseMessage(FileUpdateCompleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseHasBeenDeleted(Context context, params string[] data)
        {
            return ResponseMessage(HasBeenDeleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseHasBeenMoved(Context context, params string[] data)
        {
            return ResponseMessage(HasBeenMoved(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseHasNotPermission(Context context, params string[] data)
        {
            return ResponseMessage(HasNotPermission(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseHistoryDeleted(Context context, params string[] data)
        {
            return ResponseMessage(HistoryDeleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseImported(Context context, params string[] data)
        {
            return ResponseMessage(Imported(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseImportMax(Context context, params string[] data)
        {
            return ResponseMessage(ImportMax(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseInCompression(Context context, params string[] data)
        {
            return ResponseMessage(InCompression(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseInCopying(Context context, params string[] data)
        {
            return ResponseMessage(InCopying(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseIncorrectCurrentPassword(Context context, params string[] data)
        {
            return ResponseMessage(IncorrectCurrentPassword(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseIncorrectFileFormat(Context context, params string[] data)
        {
            return ResponseMessage(IncorrectFileFormat(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseIncorrectSiteDeleting(Context context, params string[] data)
        {
            return ResponseMessage(IncorrectSiteDeleting(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseInputMailAddress(Context context, params string[] data)
        {
            return ResponseMessage(InputMailAddress(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseInternalServerError(Context context, params string[] data)
        {
            return ResponseMessage(InternalServerError(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseInvalidCsvData(Context context, params string[] data)
        {
            return ResponseMessage(InvalidCsvData(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseInvalidFormula(Context context, params string[] data)
        {
            return ResponseMessage(InvalidFormula(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseInvalidRequest(Context context, params string[] data)
        {
            return ResponseMessage(InvalidRequest(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseItemsLimit(Context context, params string[] data)
        {
            return ResponseMessage(ItemsLimit(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseLinkCreated(Context context, params string[] data)
        {
            return ResponseMessage(LinkCreated(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseLoginIdAlreadyUse(Context context, params string[] data)
        {
            return ResponseMessage(LoginIdAlreadyUse(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseLoginIn(Context context, params string[] data)
        {
            return ResponseMessage(LoginIn(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseMailAddressHasNotSet(Context context, params string[] data)
        {
            return ResponseMessage(MailAddressHasNotSet(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseMailTransmissionCompletion(Context context, params string[] data)
        {
            return ResponseMessage(MailTransmissionCompletion(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseMoved(Context context, params string[] data)
        {
            return ResponseMessage(Moved(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseNoLinks(Context context, params string[] data)
        {
            return ResponseMessage(NoLinks(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseNotFound(Context context, params string[] data)
        {
            return ResponseMessage(NotFound(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseNotRequiredColumn(Context context, params string[] data)
        {
            return ResponseMessage(NotRequiredColumn(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseOverLimitQuantity(Context context, params string[] data)
        {
            return ResponseMessage(OverLimitQuantity(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseOverLimitSize(Context context, params string[] data)
        {
            return ResponseMessage(OverLimitSize(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseOverTenantStorageSize(Context context, params string[] data)
        {
            return ResponseMessage(OverTenantStorageSize(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseOverTotalLimitSize(Context context, params string[] data)
        {
            return ResponseMessage(OverTotalLimitSize(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseParameterSyntaxError(Context context, params string[] data)
        {
            return ResponseMessage(ParameterSyntaxError(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponsePasswordNotChanged(Context context, params string[] data)
        {
            return ResponseMessage(PasswordNotChanged(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponsePasswordPolicyViolation(Context context, params string[] data)
        {
            return ResponseMessage(PasswordPolicyViolation(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponsePasswordResetCompleted(Context context, params string[] data)
        {
            return ResponseMessage(PasswordResetCompleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponsePermissionNotSelfChange(Context context, params string[] data)
        {
            return ResponseMessage(PermissionNotSelfChange(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponsePhysicalDeleted(Context context, params string[] data)
        {
            return ResponseMessage(PhysicalDeleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseReadOnlyBecausePreviousVer(Context context, params string[] data)
        {
            return ResponseMessage(ReadOnlyBecausePreviousVer(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseRebuildingCompleted(Context context, params string[] data)
        {
            return ResponseMessage(RebuildingCompleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseRequireMailAddresses(Context context, params string[] data)
        {
            return ResponseMessage(RequireMailAddresses(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseRequireTo(Context context, params string[] data)
        {
            return ResponseMessage(RequireTo(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseRestoredFromHistory(Context context, params string[] data)
        {
            return ResponseMessage(RestoredFromHistory(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseRestricted(Context context, params string[] data)
        {
            return ResponseMessage(Restricted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseSelectFile(Context context, params string[] data)
        {
            return ResponseMessage(SelectFile(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseSelectOne(Context context, params string[] data)
        {
            return ResponseMessage(SelectOne(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseSelectTargets(Context context, params string[] data)
        {
            return ResponseMessage(SelectTargets(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseSentAcceptanceMail (Context context, params string[] data)
        {
            return ResponseMessage(SentAcceptanceMail (
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseSeparated(Context context, params string[] data)
        {
            return ResponseMessage(Separated(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseSitesCreated(Context context, params string[] data)
        {
            return ResponseMessage(SitesCreated(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseSitesLimit(Context context, params string[] data)
        {
            return ResponseMessage(SitesLimit(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseSynchronizationCompleted(Context context, params string[] data)
        {
            return ResponseMessage(SynchronizationCompleted(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseTooManyCases(Context context, params string[] data)
        {
            return ResponseMessage(TooManyCases(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseTooManyColumnCases(Context context, params string[] data)
        {
            return ResponseMessage(TooManyColumnCases(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseTooManyRowCases(Context context, params string[] data)
        {
            return ResponseMessage(TooManyRowCases(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseUnauthorized(Context context, params string[] data)
        {
            return ResponseMessage(Unauthorized(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseUpdateConflicts(Context context, params string[] data)
        {
            return ResponseMessage(UpdateConflicts(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseUpdated(Context context, params string[] data)
        {
            return ResponseMessage(Updated(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseUserDisabled(Context context, params string[] data)
        {
            return ResponseMessage(UserDisabled(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseUserLockout(Context context, params string[] data)
        {
            return ResponseMessage(UserLockout(
                context: context,
                data: data));
        }

        public static ResponseCollection ResponseUsersLimit(Context context, params string[] data)
        {
            return ResponseMessage(UsersLimit(
                context: context,
                data: data));
        }
    }
}
