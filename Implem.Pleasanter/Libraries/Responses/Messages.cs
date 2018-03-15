using Implem.Pleasanter.Libraries.Html;
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

        public static Message AlreadyAdded(params string[] data)
        {
            return Get(Displays.AlreadyAdded(data), "alert-error");
        }

        public static Message AlreadyLinked(params string[] data)
        {
            return Get(Displays.AlreadyLinked(data), "alert-error");
        }

        public static Message ApiKeyCreated(params string[] data)
        {
            return Get(Displays.ApiKeyCreated(data), "alert-success");
        }

        public static Message ApiKeyDeleted(params string[] data)
        {
            return Get(Displays.ApiKeyDeleted(data), "alert-success");
        }

        public static Message ApplicationError(params string[] data)
        {
            return Get(Displays.ApplicationError(data), "alert-error");
        }

        public static Message Authentication(params string[] data)
        {
            return Get(Displays.Authentication(data), "alert-error");
        }

        public static Message BadFormat(params string[] data)
        {
            return Get(Displays.BadFormat(data), "alert-error");
        }

        public static Message BadMailAddress(params string[] data)
        {
            return Get(Displays.BadMailAddress(data), "alert-error");
        }

        public static Message BadRequest(params string[] data)
        {
            return Get(Displays.BadRequest(data), "alert-error");
        }

        public static Message BulkDeleted(params string[] data)
        {
            return Get(Displays.BulkDeleted(data), "alert-success");
        }

        public static Message BulkMoved(params string[] data)
        {
            return Get(Displays.BulkMoved(data), "alert-success");
        }

        public static Message CanNotChangeInheritance(params string[] data)
        {
            return Get(Displays.CanNotChangeInheritance(data), "alert-error");
        }

        public static Message CanNotDisabled(params string[] data)
        {
            return Get(Displays.CanNotDisabled(data), "alert-error");
        }

        public static Message CanNotInherit(params string[] data)
        {
            return Get(Displays.CanNotInherit(data), "alert-error");
        }

        public static Message CanNotLink(params string[] data)
        {
            return Get(Displays.CanNotLink(data), "alert-error");
        }

        public static Message CanNotPerformed(params string[] data)
        {
            return Get(Displays.CanNotPerformed(data), "alert-error");
        }

        public static Message CanNotUpdate(params string[] data)
        {
            return Get(Displays.CanNotUpdate(data), "alert-info");
        }

        public static Message CantSetAtTopOfSite(params string[] data)
        {
            return Get(Displays.CantSetAtTopOfSite(data), "alert-error");
        }

        public static Message ChangingPasswordComplete(params string[] data)
        {
            return Get(Displays.ChangingPasswordComplete(data), "alert-success");
        }

        public static Message CodeDefinerBackupCompleted(params string[] data)
        {
            return Get(Displays.CodeDefinerBackupCompleted(data), "alert-success");
        }

        public static Message CodeDefinerCompleted(params string[] data)
        {
            return Get(Displays.CodeDefinerCompleted(data), "alert-success");
        }

        public static Message CodeDefinerCssCompleted(params string[] data)
        {
            return Get(Displays.CodeDefinerCssCompleted(data), "alert-success");
        }

        public static Message CodeDefinerDefCompleted(params string[] data)
        {
            return Get(Displays.CodeDefinerDefCompleted(data), "alert-success");
        }

        public static Message CodeDefinerInsertTestDataCompleted(params string[] data)
        {
            return Get(Displays.CodeDefinerInsertTestDataCompleted(data), "alert-success");
        }

        public static Message CodeDefinerMvcCompleted(params string[] data)
        {
            return Get(Displays.CodeDefinerMvcCompleted(data), "alert-success");
        }

        public static Message CodeDefinerRdsCompleted(params string[] data)
        {
            return Get(Displays.CodeDefinerRdsCompleted(data), "alert-success");
        }

        public static Message CommentDeleted(params string[] data)
        {
            return Get(Displays.CommentDeleted(data), "alert-success");
        }

        public static Message Copied(params string[] data)
        {
            return Get(Displays.Copied(data), "alert-success");
        }

        public static Message Created(params string[] data)
        {
            return Get(Displays.Created(data), "alert-success");
        }

        public static Message DefinitionNotFound(params string[] data)
        {
            return Get(Displays.DefinitionNotFound(data), "alert-error");
        }

        public static Message DeleteConflicts(params string[] data)
        {
            return Get(Displays.DeleteConflicts(data), "alert-error");
        }

        public static Message Deleted(params string[] data)
        {
            return Get(Displays.Deleted(data), "alert-success");
        }

        public static Message DeletedImage(params string[] data)
        {
            return Get(Displays.DeletedImage(data), "alert-success");
        }

        public static Message Expired(params string[] data)
        {
            return Get(Displays.Expired(data), "alert-error");
        }

        public static Message ExternalMailAddress(params string[] data)
        {
            return Get(Displays.ExternalMailAddress(data), "alert-error");
        }

        public static Message FailedReadFile(params string[] data)
        {
            return Get(Displays.FailedReadFile(data), "alert-error");
        }

        public static Message FileDeleteCompleted(params string[] data)
        {
            return Get(Displays.FileDeleteCompleted(data), "alert-success");
        }

        public static Message FileDragDrop(params string[] data)
        {
            return Get(Displays.FileDragDrop(data), "alert-success");
        }

        public static Message FileNotFound(params string[] data)
        {
            return Get(Displays.FileNotFound(data), "alert-error");
        }

        public static Message FileUpdateCompleted(params string[] data)
        {
            return Get(Displays.FileUpdateCompleted(data), "alert-success");
        }

        public static Message HasBeenDeleted(params string[] data)
        {
            return Get(Displays.HasBeenDeleted(data), "alert-info");
        }

        public static Message HasBeenMoved(params string[] data)
        {
            return Get(Displays.HasBeenMoved(data), "alert-info");
        }

        public static Message HasNotPermission(params string[] data)
        {
            return Get(Displays.HasNotPermission(data), "alert-error");
        }

        public static Message Imported(params string[] data)
        {
            return Get(Displays.Imported(data), "alert-success");
        }

        public static Message ImportMax(params string[] data)
        {
            return Get(Displays.ImportMax(data), "alert-error");
        }

        public static Message InCompression(params string[] data)
        {
            return Get(Displays.InCompression(data), "alert-info");
        }

        public static Message InCopying(params string[] data)
        {
            return Get(Displays.InCopying(data), "alert-info");
        }

        public static Message IncorrectCurrentPassword(params string[] data)
        {
            return Get(Displays.IncorrectCurrentPassword(data), "alert-error");
        }

        public static Message IncorrectFileFormat(params string[] data)
        {
            return Get(Displays.IncorrectFileFormat(data), "alert-error");
        }

        public static Message IncorrectSiteDeleting(params string[] data)
        {
            return Get(Displays.IncorrectSiteDeleting(data), "alert-error");
        }

        public static Message InputMailAddress(params string[] data)
        {
            return Get(Displays.InputMailAddress(data), "alert-error");
        }

        public static Message InternalServerError(params string[] data)
        {
            return Get(Displays.InternalServerError(data), "alert-error");
        }

        public static Message InvalidCsvData(params string[] data)
        {
            return Get(Displays.InvalidCsvData(data), "alert-error");
        }

        public static Message InvalidFormula(params string[] data)
        {
            return Get(Displays.InvalidFormula(data), "alert-error");
        }

        public static Message InvalidRequest(params string[] data)
        {
            return Get(Displays.InvalidRequest(data), "alert-error");
        }

        public static Message ItemsLimit(params string[] data)
        {
            return Get(Displays.ItemsLimit(data), "alert-error");
        }

        public static Message LinkCreated(params string[] data)
        {
            return Get(Displays.LinkCreated(data), "alert-success");
        }

        public static Message LoginIdAlreadyUse(params string[] data)
        {
            return Get(Displays.LoginIdAlreadyUse(data), "alert-error");
        }

        public static Message LoginIn(params string[] data)
        {
            return Get(Displays.LoginIn(data), "alert-success");
        }

        public static Message MailAddressHasNotSet(params string[] data)
        {
            return Get(Displays.MailAddressHasNotSet(data), "alert-error");
        }

        public static Message MailTransmissionCompletion(params string[] data)
        {
            return Get(Displays.MailTransmissionCompletion(data), "alert-success");
        }

        public static Message Moved(params string[] data)
        {
            return Get(Displays.Moved(data), "alert-success");
        }

        public static Message NoLinks(params string[] data)
        {
            return Get(Displays.NoLinks(data), "alert-error");
        }

        public static Message NotFound(params string[] data)
        {
            return Get(Displays.NotFound(data), "alert-error");
        }

        public static Message NotRequiredColumn(params string[] data)
        {
            return Get(Displays.NotRequiredColumn(data), "alert-error");
        }

        public static Message OverLimitQuantity(params string[] data)
        {
            return Get(Displays.OverLimitQuantity(data), "alert-error");
        }

        public static Message OverLimitSize(params string[] data)
        {
            return Get(Displays.OverLimitSize(data), "alert-error");
        }

        public static Message OverTenantStorageSize(params string[] data)
        {
            return Get(Displays.OverTenantStorageSize(data), "alert-error");
        }

        public static Message OverTotalLimitSize(params string[] data)
        {
            return Get(Displays.OverTotalLimitSize(data), "alert-error");
        }

        public static Message ParameterSyntaxError(params string[] data)
        {
            return Get(Displays.ParameterSyntaxError(data), "alert-error");
        }

        public static Message PasswordNotChanged(params string[] data)
        {
            return Get(Displays.PasswordNotChanged(data), "alert-error");
        }

        public static Message PasswordResetCompleted(params string[] data)
        {
            return Get(Displays.PasswordResetCompleted(data), "alert-success");
        }

        public static Message PermissionNotSelfChange(params string[] data)
        {
            return Get(Displays.PermissionNotSelfChange(data), "alert-error");
        }

        public static Message ReadOnlyBecausePreviousVer(params string[] data)
        {
            return Get(Displays.ReadOnlyBecausePreviousVer(data), "alert-info");
        }

        public static Message RequireMailAddresses(params string[] data)
        {
            return Get(Displays.RequireMailAddresses(data), "alert-error");
        }

        public static Message RequireTo(params string[] data)
        {
            return Get(Displays.RequireTo(data), "alert-error");
        }

        public static Message Restricted(params string[] data)
        {
            return Get(Displays.Restricted(data), "alert-error");
        }

        public static Message SelectFile(params string[] data)
        {
            return Get(Displays.SelectFile(data), "alert-error");
        }

        public static Message SelectOne(params string[] data)
        {
            return Get(Displays.SelectOne(data), "alert-error");
        }

        public static Message SelectTargets(params string[] data)
        {
            return Get(Displays.SelectTargets(data), "alert-error");
        }

        public static Message SentAcceptanceMail (params string[] data)
        {
            return Get(Displays.SentAcceptanceMail (data), "alert-success");
        }

        public static Message Separated(params string[] data)
        {
            return Get(Displays.Separated(data), "alert-success");
        }

        public static Message SitesCreated(params string[] data)
        {
            return Get(Displays.SitesCreated(data), "alert-success");
        }

        public static Message SitesLimit(params string[] data)
        {
            return Get(Displays.SitesLimit(data), "alert-error");
        }

        public static Message SynchronizationCompleted(params string[] data)
        {
            return Get(Displays.SynchronizationCompleted(data), "alert-success");
        }

        public static Message TooManyCases(params string[] data)
        {
            return Get(Displays.TooManyCases(data), "alert-error");
        }

        public static Message TooManyColumnCases(params string[] data)
        {
            return Get(Displays.TooManyColumnCases(data), "alert-error");
        }

        public static Message TooManyRowCases(params string[] data)
        {
            return Get(Displays.TooManyRowCases(data), "alert-error");
        }

        public static Message Unauthorized(params string[] data)
        {
            return Get(Displays.Unauthorized(data), "alert-error");
        }

        public static Message UpdateConflicts(params string[] data)
        {
            return Get(Displays.UpdateConflicts(data), "alert-error");
        }

        public static Message Updated(params string[] data)
        {
            return Get(Displays.Updated(data), "alert-success");
        }

        public static Message UsersLimit(params string[] data)
        {
            return Get(Displays.UsersLimit(data), "alert-error");
        }

        public static ResponseCollection ResponseAlreadyAdded(params string[] data)
        {
            return ResponseMessage(AlreadyAdded(data));
        }

        public static ResponseCollection ResponseAlreadyLinked(params string[] data)
        {
            return ResponseMessage(AlreadyLinked(data));
        }

        public static ResponseCollection ResponseApiKeyCreated(params string[] data)
        {
            return ResponseMessage(ApiKeyCreated(data));
        }

        public static ResponseCollection ResponseApiKeyDeleted(params string[] data)
        {
            return ResponseMessage(ApiKeyDeleted(data));
        }

        public static ResponseCollection ResponseApplicationError(params string[] data)
        {
            return ResponseMessage(ApplicationError(data));
        }

        public static ResponseCollection ResponseAuthentication(params string[] data)
        {
            return ResponseMessage(Authentication(data));
        }

        public static ResponseCollection ResponseBadFormat(params string[] data)
        {
            return ResponseMessage(BadFormat(data));
        }

        public static ResponseCollection ResponseBadMailAddress(params string[] data)
        {
            return ResponseMessage(BadMailAddress(data));
        }

        public static ResponseCollection ResponseBadRequest(params string[] data)
        {
            return ResponseMessage(BadRequest(data));
        }

        public static ResponseCollection ResponseBulkDeleted(params string[] data)
        {
            return ResponseMessage(BulkDeleted(data));
        }

        public static ResponseCollection ResponseBulkMoved(params string[] data)
        {
            return ResponseMessage(BulkMoved(data));
        }

        public static ResponseCollection ResponseCanNotChangeInheritance(params string[] data)
        {
            return ResponseMessage(CanNotChangeInheritance(data));
        }

        public static ResponseCollection ResponseCanNotDisabled(params string[] data)
        {
            return ResponseMessage(CanNotDisabled(data));
        }

        public static ResponseCollection ResponseCanNotInherit(params string[] data)
        {
            return ResponseMessage(CanNotInherit(data));
        }

        public static ResponseCollection ResponseCanNotLink(params string[] data)
        {
            return ResponseMessage(CanNotLink(data));
        }

        public static ResponseCollection ResponseCanNotPerformed(params string[] data)
        {
            return ResponseMessage(CanNotPerformed(data));
        }

        public static ResponseCollection ResponseCanNotUpdate(params string[] data)
        {
            return ResponseMessage(CanNotUpdate(data));
        }

        public static ResponseCollection ResponseCantSetAtTopOfSite(params string[] data)
        {
            return ResponseMessage(CantSetAtTopOfSite(data));
        }

        public static ResponseCollection ResponseChangingPasswordComplete(params string[] data)
        {
            return ResponseMessage(ChangingPasswordComplete(data));
        }

        public static ResponseCollection ResponseCodeDefinerBackupCompleted(params string[] data)
        {
            return ResponseMessage(CodeDefinerBackupCompleted(data));
        }

        public static ResponseCollection ResponseCodeDefinerCompleted(params string[] data)
        {
            return ResponseMessage(CodeDefinerCompleted(data));
        }

        public static ResponseCollection ResponseCodeDefinerCssCompleted(params string[] data)
        {
            return ResponseMessage(CodeDefinerCssCompleted(data));
        }

        public static ResponseCollection ResponseCodeDefinerDefCompleted(params string[] data)
        {
            return ResponseMessage(CodeDefinerDefCompleted(data));
        }

        public static ResponseCollection ResponseCodeDefinerInsertTestDataCompleted(params string[] data)
        {
            return ResponseMessage(CodeDefinerInsertTestDataCompleted(data));
        }

        public static ResponseCollection ResponseCodeDefinerMvcCompleted(params string[] data)
        {
            return ResponseMessage(CodeDefinerMvcCompleted(data));
        }

        public static ResponseCollection ResponseCodeDefinerRdsCompleted(params string[] data)
        {
            return ResponseMessage(CodeDefinerRdsCompleted(data));
        }

        public static ResponseCollection ResponseCommentDeleted(params string[] data)
        {
            return ResponseMessage(CommentDeleted(data));
        }

        public static ResponseCollection ResponseCopied(params string[] data)
        {
            return ResponseMessage(Copied(data));
        }

        public static ResponseCollection ResponseCreated(params string[] data)
        {
            return ResponseMessage(Created(data));
        }

        public static ResponseCollection ResponseDefinitionNotFound(params string[] data)
        {
            return ResponseMessage(DefinitionNotFound(data));
        }

        public static ResponseCollection ResponseDeleteConflicts(params string[] data)
        {
            return ResponseMessage(DeleteConflicts(data));
        }

        public static ResponseCollection ResponseDeleted(params string[] data)
        {
            return ResponseMessage(Deleted(data));
        }

        public static ResponseCollection ResponseDeletedImage(params string[] data)
        {
            return ResponseMessage(DeletedImage(data));
        }

        public static ResponseCollection ResponseExpired(params string[] data)
        {
            return ResponseMessage(Expired(data));
        }

        public static ResponseCollection ResponseExternalMailAddress(params string[] data)
        {
            return ResponseMessage(ExternalMailAddress(data));
        }

        public static ResponseCollection ResponseFailedReadFile(params string[] data)
        {
            return ResponseMessage(FailedReadFile(data));
        }

        public static ResponseCollection ResponseFileDeleteCompleted(params string[] data)
        {
            return ResponseMessage(FileDeleteCompleted(data));
        }

        public static ResponseCollection ResponseFileDragDrop(params string[] data)
        {
            return ResponseMessage(FileDragDrop(data));
        }

        public static ResponseCollection ResponseFileNotFound(params string[] data)
        {
            return ResponseMessage(FileNotFound(data));
        }

        public static ResponseCollection ResponseFileUpdateCompleted(params string[] data)
        {
            return ResponseMessage(FileUpdateCompleted(data));
        }

        public static ResponseCollection ResponseHasBeenDeleted(params string[] data)
        {
            return ResponseMessage(HasBeenDeleted(data));
        }

        public static ResponseCollection ResponseHasBeenMoved(params string[] data)
        {
            return ResponseMessage(HasBeenMoved(data));
        }

        public static ResponseCollection ResponseHasNotPermission(params string[] data)
        {
            return ResponseMessage(HasNotPermission(data));
        }

        public static ResponseCollection ResponseImported(params string[] data)
        {
            return ResponseMessage(Imported(data));
        }

        public static ResponseCollection ResponseImportMax(params string[] data)
        {
            return ResponseMessage(ImportMax(data));
        }

        public static ResponseCollection ResponseInCompression(params string[] data)
        {
            return ResponseMessage(InCompression(data));
        }

        public static ResponseCollection ResponseInCopying(params string[] data)
        {
            return ResponseMessage(InCopying(data));
        }

        public static ResponseCollection ResponseIncorrectCurrentPassword(params string[] data)
        {
            return ResponseMessage(IncorrectCurrentPassword(data));
        }

        public static ResponseCollection ResponseIncorrectFileFormat(params string[] data)
        {
            return ResponseMessage(IncorrectFileFormat(data));
        }

        public static ResponseCollection ResponseIncorrectSiteDeleting(params string[] data)
        {
            return ResponseMessage(IncorrectSiteDeleting(data));
        }

        public static ResponseCollection ResponseInputMailAddress(params string[] data)
        {
            return ResponseMessage(InputMailAddress(data));
        }

        public static ResponseCollection ResponseInternalServerError(params string[] data)
        {
            return ResponseMessage(InternalServerError(data));
        }

        public static ResponseCollection ResponseInvalidCsvData(params string[] data)
        {
            return ResponseMessage(InvalidCsvData(data));
        }

        public static ResponseCollection ResponseInvalidFormula(params string[] data)
        {
            return ResponseMessage(InvalidFormula(data));
        }

        public static ResponseCollection ResponseInvalidRequest(params string[] data)
        {
            return ResponseMessage(InvalidRequest(data));
        }

        public static ResponseCollection ResponseItemsLimit(params string[] data)
        {
            return ResponseMessage(ItemsLimit(data));
        }

        public static ResponseCollection ResponseLinkCreated(params string[] data)
        {
            return ResponseMessage(LinkCreated(data));
        }

        public static ResponseCollection ResponseLoginIdAlreadyUse(params string[] data)
        {
            return ResponseMessage(LoginIdAlreadyUse(data));
        }

        public static ResponseCollection ResponseLoginIn(params string[] data)
        {
            return ResponseMessage(LoginIn(data));
        }

        public static ResponseCollection ResponseMailAddressHasNotSet(params string[] data)
        {
            return ResponseMessage(MailAddressHasNotSet(data));
        }

        public static ResponseCollection ResponseMailTransmissionCompletion(params string[] data)
        {
            return ResponseMessage(MailTransmissionCompletion(data));
        }

        public static ResponseCollection ResponseMoved(params string[] data)
        {
            return ResponseMessage(Moved(data));
        }

        public static ResponseCollection ResponseNoLinks(params string[] data)
        {
            return ResponseMessage(NoLinks(data));
        }

        public static ResponseCollection ResponseNotFound(params string[] data)
        {
            return ResponseMessage(NotFound(data));
        }

        public static ResponseCollection ResponseNotRequiredColumn(params string[] data)
        {
            return ResponseMessage(NotRequiredColumn(data));
        }

        public static ResponseCollection ResponseOverLimitQuantity(params string[] data)
        {
            return ResponseMessage(OverLimitQuantity(data));
        }

        public static ResponseCollection ResponseOverLimitSize(params string[] data)
        {
            return ResponseMessage(OverLimitSize(data));
        }

        public static ResponseCollection ResponseOverTenantStorageSize(params string[] data)
        {
            return ResponseMessage(OverTenantStorageSize(data));
        }

        public static ResponseCollection ResponseOverTotalLimitSize(params string[] data)
        {
            return ResponseMessage(OverTotalLimitSize(data));
        }

        public static ResponseCollection ResponseParameterSyntaxError(params string[] data)
        {
            return ResponseMessage(ParameterSyntaxError(data));
        }

        public static ResponseCollection ResponsePasswordNotChanged(params string[] data)
        {
            return ResponseMessage(PasswordNotChanged(data));
        }

        public static ResponseCollection ResponsePasswordResetCompleted(params string[] data)
        {
            return ResponseMessage(PasswordResetCompleted(data));
        }

        public static ResponseCollection ResponsePermissionNotSelfChange(params string[] data)
        {
            return ResponseMessage(PermissionNotSelfChange(data));
        }

        public static ResponseCollection ResponseReadOnlyBecausePreviousVer(params string[] data)
        {
            return ResponseMessage(ReadOnlyBecausePreviousVer(data));
        }

        public static ResponseCollection ResponseRequireMailAddresses(params string[] data)
        {
            return ResponseMessage(RequireMailAddresses(data));
        }

        public static ResponseCollection ResponseRequireTo(params string[] data)
        {
            return ResponseMessage(RequireTo(data));
        }

        public static ResponseCollection ResponseRestricted(params string[] data)
        {
            return ResponseMessage(Restricted(data));
        }

        public static ResponseCollection ResponseSelectFile(params string[] data)
        {
            return ResponseMessage(SelectFile(data));
        }

        public static ResponseCollection ResponseSelectOne(params string[] data)
        {
            return ResponseMessage(SelectOne(data));
        }

        public static ResponseCollection ResponseSelectTargets(params string[] data)
        {
            return ResponseMessage(SelectTargets(data));
        }

        public static ResponseCollection ResponseSentAcceptanceMail (params string[] data)
        {
            return ResponseMessage(SentAcceptanceMail (data));
        }

        public static ResponseCollection ResponseSeparated(params string[] data)
        {
            return ResponseMessage(Separated(data));
        }

        public static ResponseCollection ResponseSitesCreated(params string[] data)
        {
            return ResponseMessage(SitesCreated(data));
        }

        public static ResponseCollection ResponseSitesLimit(params string[] data)
        {
            return ResponseMessage(SitesLimit(data));
        }

        public static ResponseCollection ResponseSynchronizationCompleted(params string[] data)
        {
            return ResponseMessage(SynchronizationCompleted(data));
        }

        public static ResponseCollection ResponseTooManyCases(params string[] data)
        {
            return ResponseMessage(TooManyCases(data));
        }

        public static ResponseCollection ResponseTooManyColumnCases(params string[] data)
        {
            return ResponseMessage(TooManyColumnCases(data));
        }

        public static ResponseCollection ResponseTooManyRowCases(params string[] data)
        {
            return ResponseMessage(TooManyRowCases(data));
        }

        public static ResponseCollection ResponseUnauthorized(params string[] data)
        {
            return ResponseMessage(Unauthorized(data));
        }

        public static ResponseCollection ResponseUpdateConflicts(params string[] data)
        {
            return ResponseMessage(UpdateConflicts(data));
        }

        public static ResponseCollection ResponseUpdated(params string[] data)
        {
            return ResponseMessage(Updated(data));
        }

        public static ResponseCollection ResponseUsersLimit(params string[] data)
        {
            return ResponseMessage(UsersLimit(data));
        }
    }
}
