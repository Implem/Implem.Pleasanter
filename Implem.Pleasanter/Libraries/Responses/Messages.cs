using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Requests;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class Messages
    {
        private static Message Get(string id, string text, string css)
        {
            return new Message(
                id: id,
                text: text,
                css: css);
        }

        private static ResponseCollection ResponseMessage(
            Context context,
            Message message,
            string target = null)
        {
            return new ResponseCollection(context: context).Message(
                message: message,
                target: target);
        }

        public static ResponseCollection ResponseMessage(this PasswordPolicy policy, Context context)
        {
            return new ResponseCollection(context: context).Message(policy.Languages?.Any() == true
                ? new Message(
                    id: null,
                    text: policy.Display(context: context),
                    css: "alert-error")
                : PasswordPolicyViolation(context: context));
        }

        public static Message AlreadyAdded(Context context, params string[] data)
        {
            return Get(
                id: "AlreadyAdded",
                text: Displays.AlreadyAdded(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message AlreadyLinked(Context context, params string[] data)
        {
            return Get(
                id: "AlreadyLinked",
                text: Displays.AlreadyLinked(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ApiCountReset(Context context, params string[] data)
        {
            return Get(
                id: "ApiCountReset",
                text: Displays.ApiCountReset(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message ApiKeyCreated(Context context, params string[] data)
        {
            return Get(
                id: "ApiKeyCreated",
                text: Displays.ApiKeyCreated(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message ApiKeyDeleted(Context context, params string[] data)
        {
            return Get(
                id: "ApiKeyDeleted",
                text: Displays.ApiKeyDeleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message ApplicationError(Context context, params string[] data)
        {
            return Get(
                id: "ApplicationError",
                text: Displays.ApplicationError(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ApprovalMessage(Context context, params string[] data)
        {
            return Get(
                id: "ApprovalMessage",
                text: Displays.ApprovalMessage(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message ApprovalMessageInvited(Context context, params string[] data)
        {
            return Get(
                id: "ApprovalMessageInvited",
                text: Displays.ApprovalMessageInvited(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ApprovalMessageInviting(Context context, params string[] data)
        {
            return Get(
                id: "ApprovalMessageInviting",
                text: Displays.ApprovalMessageInviting(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ApprovalRequestMessage(Context context, params string[] data)
        {
            return Get(
                id: "ApprovalRequestMessage",
                text: Displays.ApprovalRequestMessage(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message ApprovalRequestMessageRequesting(Context context, params string[] data)
        {
            return Get(
                id: "ApprovalRequestMessageRequesting",
                text: Displays.ApprovalRequestMessageRequesting(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message Authentication(Context context, params string[] data)
        {
            return Get(
                id: "Authentication",
                text: Displays.Authentication(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message BadFormat(Context context, params string[] data)
        {
            return Get(
                id: "BadFormat",
                text: Displays.BadFormat(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message BadMailAddress(Context context, params string[] data)
        {
            return Get(
                id: "BadMailAddress",
                text: Displays.BadMailAddress(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message BadPasswordWhenImporting(Context context, params string[] data)
        {
            return Get(
                id: "BadPasswordWhenImporting",
                text: Displays.BadPasswordWhenImporting(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message BadRequest(Context context, params string[] data)
        {
            return Get(
                id: "BadRequest",
                text: Displays.BadRequest(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message BulkDeleted(Context context, params string[] data)
        {
            return Get(
                id: "BulkDeleted",
                text: Displays.BulkDeleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message BulkMoved(Context context, params string[] data)
        {
            return Get(
                id: "BulkMoved",
                text: Displays.BulkMoved(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message BulkProcessed(Context context, params string[] data)
        {
            return Get(
                id: "BulkProcessed",
                text: Displays.BulkProcessed(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message BulkRestored(Context context, params string[] data)
        {
            return Get(
                id: "BulkRestored",
                text: Displays.BulkRestored(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message BulkUpdated(Context context, params string[] data)
        {
            return Get(
                id: "BulkUpdated",
                text: Displays.BulkUpdated(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CanNotChangeInheritance(Context context, params string[] data)
        {
            return Get(
                id: "CanNotChangeInheritance",
                text: Displays.CanNotChangeInheritance(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CanNotDelete(Context context, params string[] data)
        {
            return Get(
                id: "CanNotDelete",
                text: Displays.CanNotDelete(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CannotDeletePermissionInherited(Context context, params string[] data)
        {
            return Get(
                id: "CannotDeletePermissionInherited",
                text: Displays.CannotDeletePermissionInherited(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CanNotDisabled(Context context, params string[] data)
        {
            return Get(
                id: "CanNotDisabled",
                text: Displays.CanNotDisabled(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CanNotGridSort(Context context, params string[] data)
        {
            return Get(
                id: "CanNotGridSort",
                text: Displays.CanNotGridSort(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CanNotInherit(Context context, params string[] data)
        {
            return Get(
                id: "CanNotInherit",
                text: Displays.CanNotInherit(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CanNotLink(Context context, params string[] data)
        {
            return Get(
                id: "CanNotLink",
                text: Displays.CanNotLink(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CannotMoveMultipleSitesData(Context context, params string[] data)
        {
            return Get(
                id: "CannotMoveMultipleSitesData",
                text: Displays.CannotMoveMultipleSitesData(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CanNotPerformed(Context context, params string[] data)
        {
            return Get(
                id: "CanNotPerformed",
                text: Displays.CanNotPerformed(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CanNotUpdate(Context context, params string[] data)
        {
            return Get(
                id: "CanNotUpdate",
                text: Displays.CanNotUpdate(
                    context: context,
                    data: data),
                css: "alert-information");
        }

        public static Message CantSetAtTopOfSite(Context context, params string[] data)
        {
            return Get(
                id: "CantSetAtTopOfSite",
                text: Displays.CantSetAtTopOfSite(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CaptchaVerificationFailed(Context context, params string[] data)
        {
            return Get(
                id: "CaptchaVerificationFailed",
                text: Displays.CaptchaVerificationFailed(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ChangingPasswordComplete(Context context, params string[] data)
        {
            return Get(
                id: "ChangingPasswordComplete",
                text: Displays.ChangingPasswordComplete(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CircularGroupChild(Context context, params string[] data)
        {
            return Get(
                id: "CircularGroupChild",
                text: Displays.CircularGroupChild(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CodeDefinerBackupCompleted(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerBackupCompleted",
                text: Displays.CodeDefinerBackupCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerCommunityEdition(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerCommunityEdition",
                text: Displays.CodeDefinerCommunityEdition(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerCompleted(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerCompleted",
                text: Displays.CodeDefinerCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerCssCompleted(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerCssCompleted",
                text: Displays.CodeDefinerCssCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerDatabaseNotFound(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerDatabaseNotFound",
                text: Displays.CodeDefinerDatabaseNotFound(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerDefCompleted(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerDefCompleted",
                text: Displays.CodeDefinerDefCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerEnterpriseEdition(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerEnterpriseEdition",
                text: Displays.CodeDefinerEnterpriseEdition(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerEnterpriseInputYesOrNo(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerEnterpriseInputYesOrNo",
                text: Displays.CodeDefinerEnterpriseInputYesOrNo(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerErrorColumnsShrinked(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerErrorColumnsShrinked",
                text: Displays.CodeDefinerErrorColumnsShrinked(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerErrorCount(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerErrorCount",
                text: Displays.CodeDefinerErrorCount(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerInsertTestDataCompleted(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerInsertTestDataCompleted",
                text: Displays.CodeDefinerInsertTestDataCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerIssueNewLicense(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerIssueNewLicense",
                text: Displays.CodeDefinerIssueNewLicense(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerLicenseInfo(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerLicenseInfo",
                text: Displays.CodeDefinerLicenseInfo(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerMigrationCheck(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerMigrationCheck",
                text: Displays.CodeDefinerMigrationCheck(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerMigrationCheckNoChanges(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerMigrationCheckNoChanges",
                text: Displays.CodeDefinerMigrationCheckNoChanges(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerMigrationCompleted(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerMigrationCompleted",
                text: Displays.CodeDefinerMigrationCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerMigrationErrors(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerMigrationErrors",
                text: Displays.CodeDefinerMigrationErrors(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerMvcCompleted(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerMvcCompleted",
                text: Displays.CodeDefinerMvcCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerRdsCanceled(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerRdsCanceled",
                text: Displays.CodeDefinerRdsCanceled(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerRdsCompleted(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerRdsCompleted",
                text: Displays.CodeDefinerRdsCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerReducedColumnList(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerReducedColumnList",
                text: Displays.CodeDefinerReducedColumnList(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerSkipUserInput(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerSkipUserInput",
                text: Displays.CodeDefinerSkipUserInput(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerTrialInputYesOrNo(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerTrialInputYesOrNo",
                text: Displays.CodeDefinerTrialInputYesOrNo(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerTrialLicenseExpired(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerTrialLicenseExpired",
                text: Displays.CodeDefinerTrialLicenseExpired(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CodeDefinerTrialShrinked(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerTrialShrinked",
                text: Displays.CodeDefinerTrialShrinked(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CommentDeleted(Context context, params string[] data)
        {
            return Get(
                id: "CommentDeleted",
                text: Displays.CommentDeleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message Copied(Context context, params string[] data)
        {
            return Get(
                id: "Copied",
                text: Displays.Copied(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message Created(Context context, params string[] data)
        {
            return Get(
                id: "Created",
                text: Displays.Created(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message CustomAppsLimit(Context context, params string[] data)
        {
            return Get(
                id: "CustomAppsLimit",
                text: Displays.CustomAppsLimit(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message CustomError(Context context, params string[] data)
        {
            return Get(
                id: "CustomError",
                text: Displays.CustomError(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message DefinitionNotFound(Context context, params string[] data)
        {
            return Get(
                id: "DefinitionNotFound",
                text: Displays.DefinitionNotFound(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message DeleteConflicts(Context context, params string[] data)
        {
            return Get(
                id: "DeleteConflicts",
                text: Displays.DeleteConflicts(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message Deleted(Context context, params string[] data)
        {
            return Get(
                id: "Deleted",
                text: Displays.Deleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message DeletedImage(Context context, params string[] data)
        {
            return Get(
                id: "DeletedImage",
                text: Displays.DeletedImage(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message Duplicated(Context context, params string[] data)
        {
            return Get(
                id: "Duplicated",
                text: Displays.Duplicated(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message EmptyUserName(Context context, params string[] data)
        {
            return Get(
                id: "EmptyUserName",
                text: Displays.EmptyUserName(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message Expired(Context context, params string[] data)
        {
            return Get(
                id: "Expired",
                text: Displays.Expired(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ExportAccepted(Context context, params string[] data)
        {
            return Get(
                id: "ExportAccepted",
                text: Displays.ExportAccepted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message ExportNotSetEmail(Context context, params string[] data)
        {
            return Get(
                id: "ExportNotSetEmail",
                text: Displays.ExportNotSetEmail(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ExternalMailAddress(Context context, params string[] data)
        {
            return Get(
                id: "ExternalMailAddress",
                text: Displays.ExternalMailAddress(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message FailedBulkUpsert(Context context, params string[] data)
        {
            return Get(
                id: "FailedBulkUpsert",
                text: Displays.FailedBulkUpsert(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message FailedReadFile(Context context, params string[] data)
        {
            return Get(
                id: "FailedReadFile",
                text: Displays.FailedReadFile(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message FailedWriteFile(Context context, params string[] data)
        {
            return Get(
                id: "FailedWriteFile",
                text: Displays.FailedWriteFile(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message FileDeleteCompleted(Context context, params string[] data)
        {
            return Get(
                id: "FileDeleteCompleted",
                text: Displays.FileDeleteCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message FileDragDrop(Context context, params string[] data)
        {
            return Get(
                id: "FileDragDrop",
                text: Displays.FileDragDrop(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message FileNotFound(Context context, params string[] data)
        {
            return Get(
                id: "FileNotFound",
                text: Displays.FileNotFound(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message FileUpdateCompleted(Context context, params string[] data)
        {
            return Get(
                id: "FileUpdateCompleted",
                text: Displays.FileUpdateCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message GroupDepthMax(Context context, params string[] data)
        {
            return Get(
                id: "GroupDepthMax",
                text: Displays.GroupDepthMax(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message GroupImported(Context context, params string[] data)
        {
            return Get(
                id: "GroupImported",
                text: Displays.GroupImported(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message HasBeenDeleted(Context context, params string[] data)
        {
            return Get(
                id: "HasBeenDeleted",
                text: Displays.HasBeenDeleted(
                    context: context,
                    data: data),
                css: "alert-information");
        }

        public static Message HasBeenMoved(Context context, params string[] data)
        {
            return Get(
                id: "HasBeenMoved",
                text: Displays.HasBeenMoved(
                    context: context,
                    data: data),
                css: "alert-information");
        }

        public static Message HasNotChangeColumnPermission(Context context, params string[] data)
        {
            return Get(
                id: "HasNotChangeColumnPermission",
                text: Displays.HasNotChangeColumnPermission(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message HasNotPermission(Context context, params string[] data)
        {
            return Get(
                id: "HasNotPermission",
                text: Displays.HasNotPermission(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message HistoryDeleted(Context context, params string[] data)
        {
            return Get(
                id: "HistoryDeleted",
                text: Displays.HistoryDeleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message Imported(Context context, params string[] data)
        {
            return Get(
                id: "Imported",
                text: Displays.Imported(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message ImportInvalidUserIdAndLoginId(Context context, params string[] data)
        {
            return Get(
                id: "ImportInvalidUserIdAndLoginId",
                text: Displays.ImportInvalidUserIdAndLoginId(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ImportLock(Context context, params string[] data)
        {
            return Get(
                id: "ImportLock",
                text: Displays.ImportLock(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ImportMax(Context context, params string[] data)
        {
            return Get(
                id: "ImportMax",
                text: Displays.ImportMax(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InCircleInvalidToken(Context context, params string[] data)
        {
            return Get(
                id: "InCircleInvalidToken",
                text: Displays.InCircleInvalidToken(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InCompression(Context context, params string[] data)
        {
            return Get(
                id: "InCompression",
                text: Displays.InCompression(
                    context: context,
                    data: data),
                css: "alert-information");
        }

        public static Message InCopying(Context context, params string[] data)
        {
            return Get(
                id: "InCopying",
                text: Displays.InCopying(
                    context: context,
                    data: data),
                css: "alert-information");
        }

        public static Message IncorrectCurrentPassword(Context context, params string[] data)
        {
            return Get(
                id: "IncorrectCurrentPassword",
                text: Displays.IncorrectCurrentPassword(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message IncorrectFileFormat(Context context, params string[] data)
        {
            return Get(
                id: "IncorrectFileFormat",
                text: Displays.IncorrectFileFormat(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message IncorrectServerScript(Context context, params string[] data)
        {
            return Get(
                id: "IncorrectServerScript",
                text: Displays.IncorrectServerScript(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message IncorrectSiteDeleting(Context context, params string[] data)
        {
            return Get(
                id: "IncorrectSiteDeleting",
                text: Displays.IncorrectSiteDeleting(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message IncorrectUser(Context context, params string[] data)
        {
            return Get(
                id: "IncorrectUser",
                text: Displays.IncorrectUser(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InputMailAddress(Context context, params string[] data)
        {
            return Get(
                id: "InputMailAddress",
                text: Displays.InputMailAddress(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InternalServerError(Context context, params string[] data)
        {
            return Get(
                id: "InternalServerError",
                text: Displays.InternalServerError(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InvalidCsvData(Context context, params string[] data)
        {
            return Get(
                id: "InvalidCsvData",
                text: Displays.InvalidCsvData(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InvalidDateHhMmFormat(Context context, params string[] data)
        {
            return Get(
                id: "InvalidDateHhMmFormat",
                text: Displays.InvalidDateHhMmFormat(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InvalidFormula(Context context, params string[] data)
        {
            return Get(
                id: "InvalidFormula",
                text: Displays.InvalidFormula(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InvalidIpAddress(Context context, params string[] data)
        {
            return Get(
                id: "InvalidIpAddress",
                text: Displays.InvalidIpAddress(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InvalidJsonData(Context context, params string[] data)
        {
            return Get(
                id: "InvalidJsonData",
                text: Displays.InvalidJsonData(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InvalidMemberKey(Context context, params string[] data)
        {
            return Get(
                id: "InvalidMemberKey",
                text: Displays.InvalidMemberKey(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InvalidMemberType(Context context, params string[] data)
        {
            return Get(
                id: "InvalidMemberType",
                text: Displays.InvalidMemberType(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InvalidPath(Context context, params string[] data)
        {
            return Get(
                id: "InvalidPath",
                text: Displays.InvalidPath(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InvalidRequest(Context context, params string[] data)
        {
            return Get(
                id: "InvalidRequest",
                text: Displays.InvalidRequest(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InvalidSsoCode(Context context, params string[] data)
        {
            return Get(
                id: "InvalidSsoCode",
                text: Displays.InvalidSsoCode(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InvalidUpsertKey(Context context, params string[] data)
        {
            return Get(
                id: "InvalidUpsertKey",
                text: Displays.InvalidUpsertKey(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InvalidValidateRequiredCsvData(Context context, params string[] data)
        {
            return Get(
                id: "InvalidValidateRequiredCsvData",
                text: Displays.InvalidValidateRequiredCsvData(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message InviteMessage(Context context, params string[] data)
        {
            return Get(
                id: "InviteMessage",
                text: Displays.InviteMessage(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message ItemsLimit(Context context, params string[] data)
        {
            return Get(
                id: "ItemsLimit",
                text: Displays.ItemsLimit(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message JoeAccountCheck(Context context, params string[] data)
        {
            return Get(
                id: "JoeAccountCheck",
                text: Displays.JoeAccountCheck(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message LinkCreated(Context context, params string[] data)
        {
            return Get(
                id: "LinkCreated",
                text: Displays.LinkCreated(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message LockedRecord(Context context, params string[] data)
        {
            return Get(
                id: "LockedRecord",
                text: Displays.LockedRecord(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message LockedTable(Context context, params string[] data)
        {
            return Get(
                id: "LockedTable",
                text: Displays.LockedTable(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message LoginExpired(Context context, params string[] data)
        {
            return Get(
                id: "LoginExpired",
                text: Displays.LoginExpired(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message LoginIdAlreadyUse(Context context, params string[] data)
        {
            return Get(
                id: "LoginIdAlreadyUse",
                text: Displays.LoginIdAlreadyUse(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message LoginIn(Context context, params string[] data)
        {
            return Get(
                id: "LoginIn",
                text: Displays.LoginIn(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message MailAddressHasNotSet(Context context, params string[] data)
        {
            return Get(
                id: "MailAddressHasNotSet",
                text: Displays.MailAddressHasNotSet(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message MailTransmissionCompletion(Context context, params string[] data)
        {
            return Get(
                id: "MailTransmissionCompletion",
                text: Displays.MailTransmissionCompletion(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message Moved(Context context, params string[] data)
        {
            return Get(
                id: "Moved",
                text: Displays.Moved(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message NoLinks(Context context, params string[] data)
        {
            return Get(
                id: "NoLinks",
                text: Displays.NoLinks(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message NotContainKeyColumn(Context context, params string[] data)
        {
            return Get(
                id: "NotContainKeyColumn",
                text: Displays.NotContainKeyColumn(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message NotFound(Context context, params string[] data)
        {
            return Get(
                id: "NotFound",
                text: Displays.NotFound(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message NotIncludedRequiredColumn(Context context, params string[] data)
        {
            return Get(
                id: "NotIncludedRequiredColumn",
                text: Displays.NotIncludedRequiredColumn(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message NotLockedRecord(Context context, params string[] data)
        {
            return Get(
                id: "NotLockedRecord",
                text: Displays.NotLockedRecord(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message NotMatchRegex(Context context, params string[] data)
        {
            return Get(
                id: "NotMatchRegex",
                text: Displays.NotMatchRegex(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message Overlap(Context context, params string[] data)
        {
            return Get(
                id: "Overlap",
                text: Displays.Overlap(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message OverlapCsvImport(Context context, params string[] data)
        {
            return Get(
                id: "OverlapCsvImport",
                text: Displays.OverlapCsvImport(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message OverLimitApi(Context context, params string[] data)
        {
            return Get(
                id: "OverLimitApi",
                text: Displays.OverLimitApi(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message OverLimitQuantity(Context context, params string[] data)
        {
            return Get(
                id: "OverLimitQuantity",
                text: Displays.OverLimitQuantity(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message OverLimitSize(Context context, params string[] data)
        {
            return Get(
                id: "OverLimitSize",
                text: Displays.OverLimitSize(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message OverLocalFolderLimitSize(Context context, params string[] data)
        {
            return Get(
                id: "OverLocalFolderLimitSize",
                text: Displays.OverLocalFolderLimitSize(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message OverLocalFolderTotalLimitSize(Context context, params string[] data)
        {
            return Get(
                id: "OverLocalFolderTotalLimitSize",
                text: Displays.OverLocalFolderTotalLimitSize(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message OverTenantStorageSize(Context context, params string[] data)
        {
            return Get(
                id: "OverTenantStorageSize",
                text: Displays.OverTenantStorageSize(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message OverTotalLimitSize(Context context, params string[] data)
        {
            return Get(
                id: "OverTotalLimitSize",
                text: Displays.OverTotalLimitSize(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ParameterSyntaxError(Context context, params string[] data)
        {
            return Get(
                id: "ParameterSyntaxError",
                text: Displays.ParameterSyntaxError(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message PasswordHasBeenUsed(Context context, params string[] data)
        {
            return Get(
                id: "PasswordHasBeenUsed",
                text: Displays.PasswordHasBeenUsed(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message PasswordNotChanged(Context context, params string[] data)
        {
            return Get(
                id: "PasswordNotChanged",
                text: Displays.PasswordNotChanged(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message PasswordPolicyViolation(Context context, params string[] data)
        {
            return Get(
                id: "PasswordPolicyViolation",
                text: Displays.PasswordPolicyViolation(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message PasswordResetCompleted(Context context, params string[] data)
        {
            return Get(
                id: "PasswordResetCompleted",
                text: Displays.PasswordResetCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message PermissionNotSelfChange(Context context, params string[] data)
        {
            return Get(
                id: "PermissionNotSelfChange",
                text: Displays.PermissionNotSelfChange(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message PhysicalBulkDeleted(Context context, params string[] data)
        {
            return Get(
                id: "PhysicalBulkDeleted",
                text: Displays.PhysicalBulkDeleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message PhysicalBulkDeletedFromRecycleBin(Context context, params string[] data)
        {
            return Get(
                id: "PhysicalBulkDeletedFromRecycleBin",
                text: Displays.PhysicalBulkDeletedFromRecycleBin(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message PleaseInputData(Context context, params string[] data)
        {
            return Get(
                id: "PleaseInputData",
                text: Displays.PleaseInputData(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message PleaseUncheck(Context context, params string[] data)
        {
            return Get(
                id: "PleaseUncheck",
                text: Displays.PleaseUncheck(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ReadOnlyBecausePreviousVer(Context context, params string[] data)
        {
            return Get(
                id: "ReadOnlyBecausePreviousVer",
                text: Displays.ReadOnlyBecausePreviousVer(
                    context: context,
                    data: data),
                css: "alert-information");
        }

        public static Message RebuildingCompleted(Context context, params string[] data)
        {
            return Get(
                id: "RebuildingCompleted",
                text: Displays.RebuildingCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message Registered(Context context, params string[] data)
        {
            return Get(
                id: "Registered",
                text: Displays.Registered(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message RegisteredDemo(Context context, params string[] data)
        {
            return Get(
                id: "RegisteredDemo",
                text: Displays.RegisteredDemo(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message RejectNullImport(Context context, params string[] data)
        {
            return Get(
                id: "RejectNullImport",
                text: Displays.RejectNullImport(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ReminderErrorContent(Context context, params string[] data)
        {
            return Get(
                id: "ReminderErrorContent",
                text: Displays.ReminderErrorContent(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message ReminderErrorTitle(Context context, params string[] data)
        {
            return Get(
                id: "ReminderErrorTitle",
                text: Displays.ReminderErrorTitle(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message RequireMailAddresses(Context context, params string[] data)
        {
            return Get(
                id: "RequireMailAddresses",
                text: Displays.RequireMailAddresses(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message RequireManagePermission(Context context, params string[] data)
        {
            return Get(
                id: "RequireManagePermission",
                text: Displays.RequireManagePermission(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message RequireSecondAuthenticationByMail(Context context, params string[] data)
        {
            return Get(
                id: "RequireSecondAuthenticationByMail",
                text: Displays.RequireSecondAuthenticationByMail(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message RequireTo(Context context, params string[] data)
        {
            return Get(
                id: "RequireTo",
                text: Displays.RequireTo(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message RestoredFromHistory(Context context, params string[] data)
        {
            return Get(
                id: "RestoredFromHistory",
                text: Displays.RestoredFromHistory(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message Restricted(Context context, params string[] data)
        {
            return Get(
                id: "Restricted",
                text: Displays.Restricted(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message SamlLoginFailed(Context context, params string[] data)
        {
            return Get(
                id: "SamlLoginFailed",
                text: Displays.SamlLoginFailed(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message SecondaryAuthentication(Context context, params string[] data)
        {
            return Get(
                id: "SecondaryAuthentication",
                text: Displays.SecondaryAuthentication(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message SelectFile(Context context, params string[] data)
        {
            return Get(
                id: "SelectFile",
                text: Displays.SelectFile(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message SelectOne(Context context, params string[] data)
        {
            return Get(
                id: "SelectOne",
                text: Displays.SelectOne(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message SelectTargets(Context context, params string[] data)
        {
            return Get(
                id: "SelectTargets",
                text: Displays.SelectTargets(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message SentAcceptanceMail (Context context, params string[] data)
        {
            return Get(
                id: "SentAcceptanceMail ",
                text: Displays.SentAcceptanceMail (
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message Separated(Context context, params string[] data)
        {
            return Get(
                id: "Separated",
                text: Displays.Separated(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message SitePackageImported(Context context, params string[] data)
        {
            return Get(
                id: "SitePackageImported",
                text: Displays.SitePackageImported(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message SitesCreated(Context context, params string[] data)
        {
            return Get(
                id: "SitesCreated",
                text: Displays.SitesCreated(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message SitesLimit(Context context, params string[] data)
        {
            return Get(
                id: "SitesLimit",
                text: Displays.SitesLimit(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message SyncByLdapStarted(Context context, params string[] data)
        {
            return Get(
                id: "SyncByLdapStarted",
                text: Displays.SyncByLdapStarted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message SynchronizationCompleted(Context context, params string[] data)
        {
            return Get(
                id: "SynchronizationCompleted",
                text: Displays.SynchronizationCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message TooLongText(Context context, params string[] data)
        {
            return Get(
                id: "TooLongText",
                text: Displays.TooLongText(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message TooManyCases(Context context, params string[] data)
        {
            return Get(
                id: "TooManyCases",
                text: Displays.TooManyCases(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message TooManyColumnCases(Context context, params string[] data)
        {
            return Get(
                id: "TooManyColumnCases",
                text: Displays.TooManyColumnCases(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message TooManyRowCases(Context context, params string[] data)
        {
            return Get(
                id: "TooManyRowCases",
                text: Displays.TooManyRowCases(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message Unauthorized(Context context, params string[] data)
        {
            return Get(
                id: "Unauthorized",
                text: Displays.Unauthorized(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message UnlockedRecord(Context context, params string[] data)
        {
            return Get(
                id: "UnlockedRecord",
                text: Displays.UnlockedRecord(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message UpdateConflicts(Context context, params string[] data)
        {
            return Get(
                id: "UpdateConflicts",
                text: Displays.UpdateConflicts(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message Updated(Context context, params string[] data)
        {
            return Get(
                id: "Updated",
                text: Displays.Updated(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message UpdatedByGrid(Context context, params string[] data)
        {
            return Get(
                id: "UpdatedByGrid",
                text: Displays.UpdatedByGrid(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message UserDisabled(Context context, params string[] data)
        {
            return Get(
                id: "UserDisabled",
                text: Displays.UserDisabled(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message UserLockout(Context context, params string[] data)
        {
            return Get(
                id: "UserLockout",
                text: Displays.UserLockout(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message UserNotSelfDelete(Context context, params string[] data)
        {
            return Get(
                id: "UserNotSelfDelete",
                text: Displays.UserNotSelfDelete(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message UsersLimit(Context context, params string[] data)
        {
            return Get(
                id: "UsersLimit",
                text: Displays.UsersLimit(
                    context: context,
                    data: data),
                css: "alert-error");
        }

        public static Message UserSwitched(Context context, params string[] data)
        {
            return Get(
                id: "UserSwitched",
                text: Displays.UserSwitched(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static ResponseCollection ResponseAlreadyAdded(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: AlreadyAdded(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseAlreadyLinked(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: AlreadyLinked(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApiCountReset(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ApiCountReset(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApiKeyCreated(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ApiKeyCreated(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApiKeyDeleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ApiKeyDeleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApplicationError(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ApplicationError(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApprovalMessage(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ApprovalMessage(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApprovalMessageInvited(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ApprovalMessageInvited(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApprovalMessageInviting(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ApprovalMessageInviting(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApprovalRequestMessage(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ApprovalRequestMessage(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApprovalRequestMessageRequesting(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ApprovalRequestMessageRequesting(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseAuthentication(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: Authentication(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseBadFormat(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: BadFormat(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseBadMailAddress(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: BadMailAddress(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseBadPasswordWhenImporting(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: BadPasswordWhenImporting(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseBadRequest(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: BadRequest(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseBulkDeleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: BulkDeleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseBulkMoved(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: BulkMoved(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseBulkProcessed(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: BulkProcessed(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseBulkRestored(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: BulkRestored(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseBulkUpdated(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: BulkUpdated(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCanNotChangeInheritance(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CanNotChangeInheritance(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCanNotDelete(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CanNotDelete(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCannotDeletePermissionInherited(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CannotDeletePermissionInherited(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCanNotDisabled(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CanNotDisabled(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCanNotGridSort(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CanNotGridSort(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCanNotInherit(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CanNotInherit(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCanNotLink(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CanNotLink(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCannotMoveMultipleSitesData(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CannotMoveMultipleSitesData(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCanNotPerformed(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CanNotPerformed(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCanNotUpdate(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CanNotUpdate(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCantSetAtTopOfSite(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CantSetAtTopOfSite(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCaptchaVerificationFailed(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CaptchaVerificationFailed(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseChangingPasswordComplete(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ChangingPasswordComplete(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCircularGroupChild(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CircularGroupChild(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerBackupCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerBackupCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerCommunityEdition(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerCommunityEdition(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerCssCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerCssCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerDatabaseNotFound(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerDatabaseNotFound(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerDefCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerDefCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerEnterpriseEdition(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerEnterpriseEdition(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerEnterpriseInputYesOrNo(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerEnterpriseInputYesOrNo(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerErrorColumnsShrinked(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerErrorColumnsShrinked(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerErrorCount(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerErrorCount(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerInsertTestDataCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerInsertTestDataCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerIssueNewLicense(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerIssueNewLicense(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerLicenseInfo(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerLicenseInfo(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerMigrationCheck(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerMigrationCheck(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerMigrationCheckNoChanges(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerMigrationCheckNoChanges(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerMigrationCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerMigrationCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerMigrationErrors(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerMigrationErrors(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerMvcCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerMvcCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerRdsCanceled(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerRdsCanceled(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerRdsCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerRdsCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerReducedColumnList(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerReducedColumnList(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerSkipUserInput(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerSkipUserInput(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerTrialInputYesOrNo(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerTrialInputYesOrNo(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerTrialLicenseExpired(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerTrialLicenseExpired(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerTrialShrinked(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CodeDefinerTrialShrinked(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCommentDeleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CommentDeleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCopied(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: Copied(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCreated(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: Created(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCustomAppsLimit(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CustomAppsLimit(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCustomError(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: CustomError(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseDefinitionNotFound(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: DefinitionNotFound(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseDeleteConflicts(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: DeleteConflicts(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseDeleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: Deleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseDeletedImage(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: DeletedImage(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseDuplicated(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: Duplicated(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseEmptyUserName(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: EmptyUserName(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseExpired(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: Expired(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseExportAccepted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ExportAccepted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseExportNotSetEmail(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ExportNotSetEmail(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseExternalMailAddress(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ExternalMailAddress(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseFailedBulkUpsert(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: FailedBulkUpsert(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseFailedReadFile(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: FailedReadFile(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseFailedWriteFile(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: FailedWriteFile(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseFileDeleteCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: FileDeleteCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseFileDragDrop(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: FileDragDrop(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseFileNotFound(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: FileNotFound(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseFileUpdateCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: FileUpdateCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseGroupDepthMax(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: GroupDepthMax(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseGroupImported(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: GroupImported(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseHasBeenDeleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: HasBeenDeleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseHasBeenMoved(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: HasBeenMoved(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseHasNotChangeColumnPermission(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: HasNotChangeColumnPermission(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseHasNotPermission(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: HasNotPermission(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseHistoryDeleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: HistoryDeleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseImported(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: Imported(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseImportInvalidUserIdAndLoginId(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ImportInvalidUserIdAndLoginId(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseImportLock(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ImportLock(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseImportMax(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ImportMax(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInCircleInvalidToken(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InCircleInvalidToken(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInCompression(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InCompression(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInCopying(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InCopying(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseIncorrectCurrentPassword(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: IncorrectCurrentPassword(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseIncorrectFileFormat(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: IncorrectFileFormat(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseIncorrectServerScript(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: IncorrectServerScript(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseIncorrectSiteDeleting(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: IncorrectSiteDeleting(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseIncorrectUser(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: IncorrectUser(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInputMailAddress(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InputMailAddress(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInternalServerError(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InternalServerError(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidCsvData(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InvalidCsvData(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidDateHhMmFormat(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InvalidDateHhMmFormat(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidFormula(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InvalidFormula(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidIpAddress(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InvalidIpAddress(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidJsonData(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InvalidJsonData(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidMemberKey(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InvalidMemberKey(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidMemberType(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InvalidMemberType(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidPath(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InvalidPath(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidRequest(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InvalidRequest(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidSsoCode(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InvalidSsoCode(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidUpsertKey(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InvalidUpsertKey(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidValidateRequiredCsvData(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InvalidValidateRequiredCsvData(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInviteMessage(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: InviteMessage(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseItemsLimit(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ItemsLimit(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseJoeAccountCheck(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: JoeAccountCheck(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseLinkCreated(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: LinkCreated(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseLockedRecord(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: LockedRecord(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseLockedTable(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: LockedTable(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseLoginExpired(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: LoginExpired(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseLoginIdAlreadyUse(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: LoginIdAlreadyUse(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseLoginIn(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: LoginIn(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseMailAddressHasNotSet(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: MailAddressHasNotSet(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseMailTransmissionCompletion(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: MailTransmissionCompletion(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseMoved(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: Moved(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseNoLinks(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: NoLinks(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseNotContainKeyColumn(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: NotContainKeyColumn(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseNotFound(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: NotFound(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseNotIncludedRequiredColumn(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: NotIncludedRequiredColumn(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseNotLockedRecord(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: NotLockedRecord(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseNotMatchRegex(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: NotMatchRegex(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseOverlap(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: Overlap(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseOverlapCsvImport(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: OverlapCsvImport(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseOverLimitApi(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: OverLimitApi(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseOverLimitQuantity(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: OverLimitQuantity(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseOverLimitSize(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: OverLimitSize(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseOverLocalFolderLimitSize(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: OverLocalFolderLimitSize(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseOverLocalFolderTotalLimitSize(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: OverLocalFolderTotalLimitSize(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseOverTenantStorageSize(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: OverTenantStorageSize(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseOverTotalLimitSize(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: OverTotalLimitSize(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseParameterSyntaxError(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ParameterSyntaxError(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponsePasswordHasBeenUsed(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: PasswordHasBeenUsed(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponsePasswordNotChanged(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: PasswordNotChanged(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponsePasswordPolicyViolation(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: PasswordPolicyViolation(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponsePasswordResetCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: PasswordResetCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponsePermissionNotSelfChange(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: PermissionNotSelfChange(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponsePhysicalBulkDeleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: PhysicalBulkDeleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponsePhysicalBulkDeletedFromRecycleBin(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: PhysicalBulkDeletedFromRecycleBin(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponsePleaseInputData(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: PleaseInputData(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponsePleaseUncheck(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: PleaseUncheck(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseReadOnlyBecausePreviousVer(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ReadOnlyBecausePreviousVer(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseRebuildingCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: RebuildingCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseRegistered(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: Registered(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseRegisteredDemo(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: RegisteredDemo(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseRejectNullImport(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: RejectNullImport(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseReminderErrorContent(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ReminderErrorContent(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseReminderErrorTitle(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: ReminderErrorTitle(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseRequireMailAddresses(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: RequireMailAddresses(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseRequireManagePermission(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: RequireManagePermission(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseRequireSecondAuthenticationByMail(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: RequireSecondAuthenticationByMail(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseRequireTo(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: RequireTo(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseRestoredFromHistory(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: RestoredFromHistory(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseRestricted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: Restricted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSamlLoginFailed(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: SamlLoginFailed(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSecondaryAuthentication(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: SecondaryAuthentication(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSelectFile(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: SelectFile(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSelectOne(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: SelectOne(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSelectTargets(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: SelectTargets(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSentAcceptanceMail (
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: SentAcceptanceMail (
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSeparated(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: Separated(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSitePackageImported(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: SitePackageImported(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSitesCreated(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: SitesCreated(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSitesLimit(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: SitesLimit(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSyncByLdapStarted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: SyncByLdapStarted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSynchronizationCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: SynchronizationCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseTooLongText(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: TooLongText(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseTooManyCases(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: TooManyCases(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseTooManyColumnCases(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: TooManyColumnCases(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseTooManyRowCases(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: TooManyRowCases(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUnauthorized(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: Unauthorized(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUnlockedRecord(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: UnlockedRecord(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUpdateConflicts(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: UpdateConflicts(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUpdated(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: Updated(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUpdatedByGrid(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: UpdatedByGrid(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUserDisabled(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: UserDisabled(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUserLockout(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: UserLockout(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUserNotSelfDelete(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: UserNotSelfDelete(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUsersLimit(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: UsersLimit(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUserSwitched(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                context: context,
                message: UserSwitched(
                    context: context,
                    data: data),
                target: target);
        }
    }
}
