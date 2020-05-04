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

        private static ResponseCollection ResponseMessage(Message message, string target = null)
        {
            return new ResponseCollection().Message(
                message: message,
                target: target);
        }

        public static ResponseCollection ResponseMessage(this PasswordPolicy policy, Context context)
        {
            return new ResponseCollection().Message(policy.Languages?.Any() == true
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

        public static Message CanNotDisabled(Context context, params string[] data)
        {
            return Get(
                id: "CanNotDisabled",
                text: Displays.CanNotDisabled(
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
                css: "alert-info");
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

        public static Message ChangingPasswordComplete(Context context, params string[] data)
        {
            return Get(
                id: "ChangingPasswordComplete",
                text: Displays.ChangingPasswordComplete(
                    context: context,
                    data: data),
                css: "alert-success");
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

        public static Message CodeDefinerDefCompleted(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerDefCompleted",
                text: Displays.CodeDefinerDefCompleted(
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

        public static Message CodeDefinerMvcCompleted(Context context, params string[] data)
        {
            return Get(
                id: "CodeDefinerMvcCompleted",
                text: Displays.CodeDefinerMvcCompleted(
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

        public static Message FailedReadFile(Context context, params string[] data)
        {
            return Get(
                id: "FailedReadFile",
                text: Displays.FailedReadFile(
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

        public static Message HasBeenDeleted(Context context, params string[] data)
        {
            return Get(
                id: "HasBeenDeleted",
                text: Displays.HasBeenDeleted(
                    context: context,
                    data: data),
                css: "alert-info");
        }

        public static Message HasBeenMoved(Context context, params string[] data)
        {
            return Get(
                id: "HasBeenMoved",
                text: Displays.HasBeenMoved(
                    context: context,
                    data: data),
                css: "alert-info");
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

        public static Message ImportMax(Context context, params string[] data)
        {
            return Get(
                id: "ImportMax",
                text: Displays.ImportMax(
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
                css: "alert-info");
        }

        public static Message InCopying(Context context, params string[] data)
        {
            return Get(
                id: "InCopying",
                text: Displays.InCopying(
                    context: context,
                    data: data),
                css: "alert-info");
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

        public static Message IncorrectSiteDeleting(Context context, params string[] data)
        {
            return Get(
                id: "IncorrectSiteDeleting",
                text: Displays.IncorrectSiteDeleting(
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

        public static Message NotFound(Context context, params string[] data)
        {
            return Get(
                id: "NotFound",
                text: Displays.NotFound(
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

        public static Message NotRequiredColumn(Context context, params string[] data)
        {
            return Get(
                id: "NotRequiredColumn",
                text: Displays.NotRequiredColumn(
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

        public static Message PhysicalDeleted(Context context, params string[] data)
        {
            return Get(
                id: "PhysicalDeleted",
                text: Displays.PhysicalDeleted(
                    context: context,
                    data: data),
                css: "alert-success");
        }

        public static Message ReadOnlyBecausePreviousVer(Context context, params string[] data)
        {
            return Get(
                id: "ReadOnlyBecausePreviousVer",
                text: Displays.ReadOnlyBecausePreviousVer(
                    context: context,
                    data: data),
                css: "alert-info");
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

        public static Message SynchronizationCompleted(Context context, params string[] data)
        {
            return Get(
                id: "SynchronizationCompleted",
                text: Displays.SynchronizationCompleted(
                    context: context,
                    data: data),
                css: "alert-success");
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
                message: AlreadyAdded(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseAlreadyLinked(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: AlreadyLinked(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApiKeyCreated(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: ApiKeyCreated(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApiKeyDeleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: ApiKeyDeleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApplicationError(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: ApplicationError(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApprovalMessage(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: ApprovalMessage(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApprovalMessageInvited(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: ApprovalMessageInvited(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApprovalMessageInviting(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: ApprovalMessageInviting(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApprovalRequestMessage(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: ApprovalRequestMessage(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseApprovalRequestMessageRequesting(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: ApprovalRequestMessageRequesting(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseAuthentication(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: Authentication(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseBadFormat(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: BadFormat(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseBadMailAddress(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: BadMailAddress(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseBadRequest(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: BadRequest(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseBulkDeleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: BulkDeleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseBulkMoved(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: BulkMoved(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseBulkRestored(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: BulkRestored(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseBulkUpdated(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: BulkUpdated(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCanNotChangeInheritance(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: CanNotChangeInheritance(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCanNotDisabled(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: CanNotDisabled(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCanNotInherit(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: CanNotInherit(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCanNotLink(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: CanNotLink(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCanNotPerformed(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: CanNotPerformed(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCanNotUpdate(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: CanNotUpdate(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCantSetAtTopOfSite(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: CantSetAtTopOfSite(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseChangingPasswordComplete(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: ChangingPasswordComplete(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerBackupCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: CodeDefinerBackupCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: CodeDefinerCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerCssCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: CodeDefinerCssCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerDefCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: CodeDefinerDefCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerErrorCount(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: CodeDefinerErrorCount(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerInsertTestDataCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: CodeDefinerInsertTestDataCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerMvcCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: CodeDefinerMvcCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCodeDefinerRdsCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: CodeDefinerRdsCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCommentDeleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: CommentDeleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCopied(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: Copied(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseCreated(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: Created(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseDefinitionNotFound(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: DefinitionNotFound(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseDeleteConflicts(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: DeleteConflicts(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseDeleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: Deleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseDeletedImage(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: DeletedImage(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseDuplicated(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: Duplicated(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseEmptyUserName(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: EmptyUserName(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseExpired(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: Expired(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseExportAccepted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: ExportAccepted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseExportNotSetEmail(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: ExportNotSetEmail(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseExternalMailAddress(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: ExternalMailAddress(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseFailedReadFile(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: FailedReadFile(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseFileDeleteCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: FileDeleteCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseFileDragDrop(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: FileDragDrop(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseFileNotFound(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: FileNotFound(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseFileUpdateCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: FileUpdateCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseHasBeenDeleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: HasBeenDeleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseHasBeenMoved(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: HasBeenMoved(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseHasNotPermission(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: HasNotPermission(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseHistoryDeleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: HistoryDeleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseImported(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: Imported(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseImportMax(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: ImportMax(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInCompression(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: InCompression(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInCopying(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: InCopying(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseIncorrectCurrentPassword(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: IncorrectCurrentPassword(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseIncorrectFileFormat(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: IncorrectFileFormat(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseIncorrectSiteDeleting(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: IncorrectSiteDeleting(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInputMailAddress(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: InputMailAddress(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInternalServerError(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: InternalServerError(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidCsvData(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: InvalidCsvData(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidFormula(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: InvalidFormula(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidIpAddress(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: InvalidIpAddress(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidRequest(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: InvalidRequest(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInvalidSsoCode(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: InvalidSsoCode(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseInviteMessage(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: InviteMessage(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseItemsLimit(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: ItemsLimit(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseJoeAccountCheck(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: JoeAccountCheck(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseLinkCreated(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: LinkCreated(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseLockedRecord(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: LockedRecord(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseLockedTable(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: LockedTable(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseLoginIdAlreadyUse(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: LoginIdAlreadyUse(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseLoginIn(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: LoginIn(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseMailAddressHasNotSet(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: MailAddressHasNotSet(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseMailTransmissionCompletion(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: MailTransmissionCompletion(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseMoved(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: Moved(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseNoLinks(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: NoLinks(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseNotFound(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: NotFound(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseNotLockedRecord(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: NotLockedRecord(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseNotRequiredColumn(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: NotRequiredColumn(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseOverLimitApi(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: OverLimitApi(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseOverLimitQuantity(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: OverLimitQuantity(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseOverLimitSize(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: OverLimitSize(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseOverLocalFolderLimitSize(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: OverLocalFolderLimitSize(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseOverLocalFolderTotalLimitSize(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: OverLocalFolderTotalLimitSize(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseOverTenantStorageSize(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: OverTenantStorageSize(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseOverTotalLimitSize(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: OverTotalLimitSize(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseParameterSyntaxError(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: ParameterSyntaxError(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponsePasswordNotChanged(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: PasswordNotChanged(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponsePasswordPolicyViolation(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: PasswordPolicyViolation(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponsePasswordResetCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: PasswordResetCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponsePermissionNotSelfChange(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: PermissionNotSelfChange(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponsePhysicalDeleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: PhysicalDeleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseReadOnlyBecausePreviousVer(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: ReadOnlyBecausePreviousVer(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseRebuildingCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: RebuildingCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseRequireMailAddresses(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: RequireMailAddresses(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseRequireManagePermission(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: RequireManagePermission(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseRequireTo(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: RequireTo(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseRestoredFromHistory(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: RestoredFromHistory(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseRestricted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: Restricted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSamlLoginFailed(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: SamlLoginFailed(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSelectFile(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: SelectFile(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSelectOne(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: SelectOne(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSelectTargets(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: SelectTargets(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSentAcceptanceMail (
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: SentAcceptanceMail (
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSeparated(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: Separated(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSitePackageImported(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: SitePackageImported(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSitesCreated(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: SitesCreated(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSitesLimit(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: SitesLimit(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseSynchronizationCompleted(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: SynchronizationCompleted(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseTooManyCases(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: TooManyCases(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseTooManyColumnCases(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: TooManyColumnCases(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseTooManyRowCases(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: TooManyRowCases(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUnauthorized(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: Unauthorized(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUnlockedRecord(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: UnlockedRecord(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUpdateConflicts(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: UpdateConflicts(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUpdated(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: Updated(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUpdatedByGrid(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: UpdatedByGrid(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUserDisabled(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: UserDisabled(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUserLockout(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: UserLockout(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUserNotSelfDelete(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: UserNotSelfDelete(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUsersLimit(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: UsersLimit(
                    context: context,
                    data: data),
                target: target);
        }

        public static ResponseCollection ResponseUserSwitched(
            Context context, string target = null, params string[] data)
        {
            return ResponseMessage(
                message: UserSwitched(
                    context: context,
                    data: data),
                target: target);
        }
    }
}
