using Implem.Pleasanter.Libraries.Views;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class Messages
    {
        private static Message Get(string text, string status)
        {
            var hb = Html.Builder();
            return new Message(
                hb.Span(css: status, action: () => hb
                    .Text(text: text)).ToString(),
                status);
        }

        private static ResponseCollection ResponseMessage(Message message)
        {
            return new ResponseCollection().Message(message);
        }

        public static Message ExceptionTitle(params string[] data)
        {
            return Get(Displays.ExceptionTitle(data), "alert-error");
        }

        public static Message ExceptionBody(params string[] data)
        {
            return Get(Displays.ExceptionBody(data), "alert-error");
        }

        public static Message IncorrectArgument(params string[] data)
        {
            return Get(Displays.IncorrectArgument(data), "alert-error");
        }

        public static Message InvalidRequest(params string[] data)
        {
            return Get(Displays.InvalidRequest(data), "alert-error");
        }

        public static Message Authentication(params string[] data)
        {
            return Get(Displays.Authentication(data), "alert-error");
        }

        public static Message UpdateConflicts(params string[] data)
        {
            return Get(Displays.UpdateConflicts(data), "alert-error");
        }

        public static Message DeleteConflicts(params string[] data)
        {
            return Get(Displays.DeleteConflicts(data), "alert-error");
        }

        public static Message HasNotPermission(params string[] data)
        {
            return Get(Displays.HasNotPermission(data), "alert-error");
        }

        public static Message IncorrectCurrentPassword(params string[] data)
        {
            return Get(Displays.IncorrectCurrentPassword(data), "alert-error");
        }

        public static Message PermissionNotSelfChange(params string[] data)
        {
            return Get(Displays.PermissionNotSelfChange(data), "alert-error");
        }

        public static Message DefinitionNotFound(params string[] data)
        {
            return Get(Displays.DefinitionNotFound(data), "alert-error");
        }

        public static Message CantSetAtTopOfSite(params string[] data)
        {
            return Get(Displays.CantSetAtTopOfSite(data), "alert-error");
        }

        public static Message NotFound(params string[] data)
        {
            return Get(Displays.NotFound(data), "alert-error");
        }

        public static Message RequireMailAddresses(params string[] data)
        {
            return Get(Displays.RequireMailAddresses(data), "alert-error");
        }

        public static Message RequireColumn(params string[] data)
        {
            return Get(Displays.RequireColumn(data), "alert-error");
        }

        public static Message ExternalMailAddress(params string[] data)
        {
            return Get(Displays.ExternalMailAddress(data), "alert-error");
        }

        public static Message BadMailAddress(params string[] data)
        {
            return Get(Displays.BadMailAddress(data), "alert-error");
        }

        public static Message FileNotFound(params string[] data)
        {
            return Get(Displays.FileNotFound(data), "alert-error");
        }

        public static Message NotRequiredColumn(params string[] data)
        {
            return Get(Displays.NotRequiredColumn(data), "alert-error");
        }

        public static Message InvalidCsvData(params string[] data)
        {
            return Get(Displays.InvalidCsvData(data), "alert-error");
        }

        public static Message FailedReadFile(params string[] data)
        {
            return Get(Displays.FailedReadFile(data), "alert-error");
        }

        public static Message CanNotHide(params string[] data)
        {
            return Get(Displays.CanNotHide(data), "alert-error");
        }

        public static Message AlreadyAdded(params string[] data)
        {
            return Get(Displays.AlreadyAdded(data), "alert-error");
        }

        public static Message SelectTargets(params string[] data)
        {
            return Get(Displays.SelectTargets(data), "alert-warning");
        }

        public static Message LoginIn(params string[] data)
        {
            return Get(Displays.LoginIn(data), "alert-success");
        }

        public static Message Deleted(params string[] data)
        {
            return Get(Displays.Deleted(data), "alert-success");
        }

        public static Message BulkMoved(params string[] data)
        {
            return Get(Displays.BulkMoved(data), "alert-success");
        }

        public static Message BulkDeleted(params string[] data)
        {
            return Get(Displays.BulkDeleted(data), "alert-success");
        }

        public static Message Separated(params string[] data)
        {
            return Get(Displays.Separated(data), "alert-success");
        }

        public static Message Imported(params string[] data)
        {
            return Get(Displays.Imported(data), "alert-success");
        }

        public static Message Created(params string[] data)
        {
            return Get(Displays.Created(data), "alert-success");
        }

        public static Message Updated(params string[] data)
        {
            return Get(Displays.Updated(data), "alert-success");
        }

        public static Message CodeDefinerCompleted(params string[] data)
        {
            return Get(Displays.CodeDefinerCompleted(data), "alert-success");
        }

        public static Message CodeDefinerDbCompleted(params string[] data)
        {
            return Get(Displays.CodeDefinerDbCompleted(data), "alert-success");
        }

        public static Message CodeDefinerDefCompleted(params string[] data)
        {
            return Get(Displays.CodeDefinerDefCompleted(data), "alert-success");
        }

        public static Message CodeDefinerMvcCompleted(params string[] data)
        {
            return Get(Displays.CodeDefinerMvcCompleted(data), "alert-success");
        }

        public static Message CodeDefinerCssCompleted(params string[] data)
        {
            return Get(Displays.CodeDefinerCssCompleted(data), "alert-success");
        }

        public static Message CodeDefinerBackupCompleted(params string[] data)
        {
            return Get(Displays.CodeDefinerBackupCompleted(data), "alert-success");
        }

        public static Message MailTransmissionCompletion(params string[] data)
        {
            return Get(Displays.MailTransmissionCompletion(data), "alert-success");
        }

        public static Message ChangingPasswordComplete(params string[] data)
        {
            return Get(Displays.ChangingPasswordComplete(data), "alert-success");
        }

        public static Message PasswordResetCompleted(params string[] data)
        {
            return Get(Displays.PasswordResetCompleted(data), "alert-success");
        }

        public static Message FileUpdateCompleted(params string[] data)
        {
            return Get(Displays.FileUpdateCompleted(data), "alert-success");
        }

        public static Message SynchronizationCompleted(params string[] data)
        {
            return Get(Displays.SynchronizationCompleted(data), "alert-success");
        }

        public static Message ConfirmDelete(params string[] data)
        {
            return Get(Displays.ConfirmDelete(data), "alert-confirm");
        }

        public static Message ConfirmSeparate(params string[] data)
        {
            return Get(Displays.ConfirmSeparate(data), "alert-confirm");
        }

        public static Message ConfirmSendMail(params string[] data)
        {
            return Get(Displays.ConfirmSendMail(data), "alert-confirm");
        }

        public static Message ConfirmSynchronize(params string[] data)
        {
            return Get(Displays.ConfirmSynchronize(data), "alert-confirm");
        }

        public static Message CanNotUpdate(params string[] data)
        {
            return Get(Displays.CanNotUpdate(data), "alert-info");
        }

        public static Message ReadOnlyBecausePreviousVer(params string[] data)
        {
            return Get(Displays.ReadOnlyBecausePreviousVer(data), "alert-info");
        }

        public static Message InCopying(params string[] data)
        {
            return Get(Displays.InCopying(data), "alert-info");
        }

        public static Message InCompression(params string[] data)
        {
            return Get(Displays.InCompression(data), "alert-info");
        }

        public static Message HasBeenMoved(params string[] data)
        {
            return Get(Displays.HasBeenMoved(data), "alert-info");
        }

        public static Message HasBeenDeleted(params string[] data)
        {
            return Get(Displays.HasBeenDeleted(data), "alert-info");
        }

        public static ResponseCollection ResponseExceptionTitle(params string[] data)
        {
            return ResponseMessage(Messages.ExceptionTitle(data));
        }

        public static ResponseCollection ResponseExceptionBody(params string[] data)
        {
            return ResponseMessage(Messages.ExceptionBody(data));
        }

        public static ResponseCollection ResponseIncorrectArgument(params string[] data)
        {
            return ResponseMessage(Messages.IncorrectArgument(data));
        }

        public static ResponseCollection ResponseInvalidRequest(params string[] data)
        {
            return ResponseMessage(Messages.InvalidRequest(data));
        }

        public static ResponseCollection ResponseAuthentication(params string[] data)
        {
            return ResponseMessage(Messages.Authentication(data));
        }

        public static ResponseCollection ResponseUpdateConflicts(params string[] data)
        {
            return ResponseMessage(Messages.UpdateConflicts(data));
        }

        public static ResponseCollection ResponseDeleteConflicts(params string[] data)
        {
            return ResponseMessage(Messages.DeleteConflicts(data));
        }

        public static ResponseCollection ResponseHasNotPermission(params string[] data)
        {
            return ResponseMessage(Messages.HasNotPermission(data));
        }

        public static ResponseCollection ResponseIncorrectCurrentPassword(params string[] data)
        {
            return ResponseMessage(Messages.IncorrectCurrentPassword(data));
        }

        public static ResponseCollection ResponsePermissionNotSelfChange(params string[] data)
        {
            return ResponseMessage(Messages.PermissionNotSelfChange(data));
        }

        public static ResponseCollection ResponseDefinitionNotFound(params string[] data)
        {
            return ResponseMessage(Messages.DefinitionNotFound(data));
        }

        public static ResponseCollection ResponseCantSetAtTopOfSite(params string[] data)
        {
            return ResponseMessage(Messages.CantSetAtTopOfSite(data));
        }

        public static ResponseCollection ResponseNotFound(params string[] data)
        {
            return ResponseMessage(Messages.NotFound(data));
        }

        public static ResponseCollection ResponseRequireMailAddresses(params string[] data)
        {
            return ResponseMessage(Messages.RequireMailAddresses(data));
        }

        public static ResponseCollection ResponseRequireColumn(params string[] data)
        {
            return ResponseMessage(Messages.RequireColumn(data));
        }

        public static ResponseCollection ResponseExternalMailAddress(params string[] data)
        {
            return ResponseMessage(Messages.ExternalMailAddress(data));
        }

        public static ResponseCollection ResponseBadMailAddress(params string[] data)
        {
            return ResponseMessage(Messages.BadMailAddress(data));
        }

        public static ResponseCollection ResponseFileNotFound(params string[] data)
        {
            return ResponseMessage(Messages.FileNotFound(data));
        }

        public static ResponseCollection ResponseNotRequiredColumn(params string[] data)
        {
            return ResponseMessage(Messages.NotRequiredColumn(data));
        }

        public static ResponseCollection ResponseInvalidCsvData(params string[] data)
        {
            return ResponseMessage(Messages.InvalidCsvData(data));
        }

        public static ResponseCollection ResponseFailedReadFile(params string[] data)
        {
            return ResponseMessage(Messages.FailedReadFile(data));
        }

        public static ResponseCollection ResponseCanNotHide(params string[] data)
        {
            return ResponseMessage(Messages.CanNotHide(data));
        }

        public static ResponseCollection ResponseAlreadyAdded(params string[] data)
        {
            return ResponseMessage(Messages.AlreadyAdded(data));
        }

        public static ResponseCollection ResponseSelectTargets(params string[] data)
        {
            return ResponseMessage(Messages.SelectTargets(data));
        }

        public static ResponseCollection ResponseLoginIn(params string[] data)
        {
            return ResponseMessage(Messages.LoginIn(data));
        }

        public static ResponseCollection ResponseDeleted(params string[] data)
        {
            return ResponseMessage(Messages.Deleted(data));
        }

        public static ResponseCollection ResponseBulkMoved(params string[] data)
        {
            return ResponseMessage(Messages.BulkMoved(data));
        }

        public static ResponseCollection ResponseBulkDeleted(params string[] data)
        {
            return ResponseMessage(Messages.BulkDeleted(data));
        }

        public static ResponseCollection ResponseSeparated(params string[] data)
        {
            return ResponseMessage(Messages.Separated(data));
        }

        public static ResponseCollection ResponseImported(params string[] data)
        {
            return ResponseMessage(Messages.Imported(data));
        }

        public static ResponseCollection ResponseCreated(params string[] data)
        {
            return ResponseMessage(Messages.Created(data));
        }

        public static ResponseCollection ResponseUpdated(params string[] data)
        {
            return ResponseMessage(Messages.Updated(data));
        }

        public static ResponseCollection ResponseCodeDefinerCompleted(params string[] data)
        {
            return ResponseMessage(Messages.CodeDefinerCompleted(data));
        }

        public static ResponseCollection ResponseCodeDefinerDbCompleted(params string[] data)
        {
            return ResponseMessage(Messages.CodeDefinerDbCompleted(data));
        }

        public static ResponseCollection ResponseCodeDefinerDefCompleted(params string[] data)
        {
            return ResponseMessage(Messages.CodeDefinerDefCompleted(data));
        }

        public static ResponseCollection ResponseCodeDefinerMvcCompleted(params string[] data)
        {
            return ResponseMessage(Messages.CodeDefinerMvcCompleted(data));
        }

        public static ResponseCollection ResponseCodeDefinerCssCompleted(params string[] data)
        {
            return ResponseMessage(Messages.CodeDefinerCssCompleted(data));
        }

        public static ResponseCollection ResponseCodeDefinerBackupCompleted(params string[] data)
        {
            return ResponseMessage(Messages.CodeDefinerBackupCompleted(data));
        }

        public static ResponseCollection ResponseMailTransmissionCompletion(params string[] data)
        {
            return ResponseMessage(Messages.MailTransmissionCompletion(data));
        }

        public static ResponseCollection ResponseChangingPasswordComplete(params string[] data)
        {
            return ResponseMessage(Messages.ChangingPasswordComplete(data));
        }

        public static ResponseCollection ResponsePasswordResetCompleted(params string[] data)
        {
            return ResponseMessage(Messages.PasswordResetCompleted(data));
        }

        public static ResponseCollection ResponseFileUpdateCompleted(params string[] data)
        {
            return ResponseMessage(Messages.FileUpdateCompleted(data));
        }

        public static ResponseCollection ResponseSynchronizationCompleted(params string[] data)
        {
            return ResponseMessage(Messages.SynchronizationCompleted(data));
        }

        public static ResponseCollection ResponseConfirmDelete(params string[] data)
        {
            return ResponseMessage(Messages.ConfirmDelete(data));
        }

        public static ResponseCollection ResponseConfirmSeparate(params string[] data)
        {
            return ResponseMessage(Messages.ConfirmSeparate(data));
        }

        public static ResponseCollection ResponseConfirmSendMail(params string[] data)
        {
            return ResponseMessage(Messages.ConfirmSendMail(data));
        }

        public static ResponseCollection ResponseConfirmSynchronize(params string[] data)
        {
            return ResponseMessage(Messages.ConfirmSynchronize(data));
        }

        public static ResponseCollection ResponseCanNotUpdate(params string[] data)
        {
            return ResponseMessage(Messages.CanNotUpdate(data));
        }

        public static ResponseCollection ResponseReadOnlyBecausePreviousVer(params string[] data)
        {
            return ResponseMessage(Messages.ReadOnlyBecausePreviousVer(data));
        }

        public static ResponseCollection ResponseInCopying(params string[] data)
        {
            return ResponseMessage(Messages.InCopying(data));
        }

        public static ResponseCollection ResponseInCompression(params string[] data)
        {
            return ResponseMessage(Messages.InCompression(data));
        }

        public static ResponseCollection ResponseHasBeenMoved(params string[] data)
        {
            return ResponseMessage(Messages.HasBeenMoved(data));
        }

        public static ResponseCollection ResponseHasBeenDeleted(params string[] data)
        {
            return ResponseMessage(Messages.HasBeenDeleted(data));
        }
    }
}
