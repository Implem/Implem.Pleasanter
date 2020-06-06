using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
namespace Implem.Pleasanter.Libraries.General
{
    public static class Error
    {
        public enum Types
        {
            None,
            AlreadyAdded,
            AlreadyLinked,
            ApplicationError,
            ApprovalMessageInvited,
            ApprovalMessageInviting,
            Authentication,
            BadFormat,
            BadMailAddress,
            BadRequest,
            CanNotChangeInheritance,
            CanNotDelete,
            CanNotDisabled,
            CanNotInherit,
            CanNotLink,
            CanNotPerformed,
            CantSetAtTopOfSite,
            DefinitionNotFound,
            DeleteConflicts,
            Duplicated,
            EmptyUserName,
            Expired,
            ExportNotSetEmail,
            ExternalMailAddress,
            FailedReadFile,
            FileNotFound,
            HasNotPermission,
            ImportMax,
            IncorrectCurrentPassword,
            IncorrectFileFormat,
            IncorrectSiteDeleting,
            InputMailAddress,
            InternalServerError,
            InvalidCsvData,
            InvalidFormula,
            InvalidIpAddress,
            InvalidRequest,
            InvalidSsoCode,
            ItemsLimit,
            JoeAccountCheck,
            LockedRecord,
            LockedTable,
            LoginIdAlreadyUse,
            MailAddressHasNotSet,
            NoLinks,
            NotFound,
            NotLockedRecord,
            NotMatchRegex,
            NotRequiredColumn,
            OverLimitApi,
            OverLimitQuantity,
            OverLimitSize,
            OverLocalFolderLimitSize,
            OverLocalFolderTotalLimitSize,
            OverTenantStorageSize,
            OverTotalLimitSize,
            ParameterSyntaxError,
            PasswordNotChanged,
            PasswordPolicyViolation,
            PermissionNotSelfChange,
            RequireMailAddresses,
            RequireManagePermission,
            RequireTo,
            Restricted,
            SamlLoginFailed,
            SelectFile,
            SelectOne,
            SelectTargets,
            SitesLimit,
            TooLongText,
            TooManyCases,
            TooManyColumnCases,
            TooManyRowCases,
            Unauthorized,
            UpdateConflicts,
            UserDisabled,
            UserLockout,
            UserNotSelfDelete,
            UsersLimit
        }

        public static bool Has(this Types type)
        {
            return type != Types.None;
        }

        public static Message Message(this Types type, Context context, params string[] data)
        {
            switch (type)
            {
                case Types.AlreadyAdded:
                    return Messages.AlreadyAdded(
                        context: context,
                        data: data);
                case Types.AlreadyLinked:
                    return Messages.AlreadyLinked(
                        context: context,
                        data: data);
                case Types.ApplicationError:
                    return Messages.ApplicationError(
                        context: context,
                        data: data);
                case Types.ApprovalMessageInvited:
                    return Messages.ApprovalMessageInvited(
                        context: context,
                        data: data);
                case Types.ApprovalMessageInviting:
                    return Messages.ApprovalMessageInviting(
                        context: context,
                        data: data);
                case Types.Authentication:
                    return Messages.Authentication(
                        context: context,
                        data: data);
                case Types.BadFormat:
                    return Messages.BadFormat(
                        context: context,
                        data: data);
                case Types.BadMailAddress:
                    return Messages.BadMailAddress(
                        context: context,
                        data: data);
                case Types.BadRequest:
                    return Messages.BadRequest(
                        context: context,
                        data: data);
                case Types.CanNotChangeInheritance:
                    return Messages.CanNotChangeInheritance(
                        context: context,
                        data: data);
                case Types.CanNotDelete:
                    return Messages.CanNotDelete(
                        context: context,
                        data: data);
                case Types.CanNotDisabled:
                    return Messages.CanNotDisabled(
                        context: context,
                        data: data);
                case Types.CanNotInherit:
                    return Messages.CanNotInherit(
                        context: context,
                        data: data);
                case Types.CanNotLink:
                    return Messages.CanNotLink(
                        context: context,
                        data: data);
                case Types.CanNotPerformed:
                    return Messages.CanNotPerformed(
                        context: context,
                        data: data);
                case Types.CantSetAtTopOfSite:
                    return Messages.CantSetAtTopOfSite(
                        context: context,
                        data: data);
                case Types.DefinitionNotFound:
                    return Messages.DefinitionNotFound(
                        context: context,
                        data: data);
                case Types.DeleteConflicts:
                    return Messages.DeleteConflicts(
                        context: context,
                        data: data);
                case Types.Duplicated:
                    return Messages.Duplicated(
                        context: context,
                        data: data);
                case Types.EmptyUserName:
                    return Messages.EmptyUserName(
                        context: context,
                        data: data);
                case Types.Expired:
                    return Messages.Expired(
                        context: context,
                        data: data);
                case Types.ExportNotSetEmail:
                    return Messages.ExportNotSetEmail(
                        context: context,
                        data: data);
                case Types.ExternalMailAddress:
                    return Messages.ExternalMailAddress(
                        context: context,
                        data: data);
                case Types.FailedReadFile:
                    return Messages.FailedReadFile(
                        context: context,
                        data: data);
                case Types.FileNotFound:
                    return Messages.FileNotFound(
                        context: context,
                        data: data);
                case Types.HasNotPermission:
                    return Messages.HasNotPermission(
                        context: context,
                        data: data);
                case Types.ImportMax:
                    return Messages.ImportMax(
                        context: context,
                        data: data);
                case Types.IncorrectCurrentPassword:
                    return Messages.IncorrectCurrentPassword(
                        context: context,
                        data: data);
                case Types.IncorrectFileFormat:
                    return Messages.IncorrectFileFormat(
                        context: context,
                        data: data);
                case Types.IncorrectSiteDeleting:
                    return Messages.IncorrectSiteDeleting(
                        context: context,
                        data: data);
                case Types.InputMailAddress:
                    return Messages.InputMailAddress(
                        context: context,
                        data: data);
                case Types.InternalServerError:
                    return Messages.InternalServerError(
                        context: context,
                        data: data);
                case Types.InvalidCsvData:
                    return Messages.InvalidCsvData(
                        context: context,
                        data: data);
                case Types.InvalidFormula:
                    return Messages.InvalidFormula(
                        context: context,
                        data: data);
                case Types.InvalidIpAddress:
                    return Messages.InvalidIpAddress(
                        context: context,
                        data: data);
                case Types.InvalidRequest:
                    return Messages.InvalidRequest(
                        context: context,
                        data: data);
                case Types.InvalidSsoCode:
                    return Messages.InvalidSsoCode(
                        context: context,
                        data: data);
                case Types.ItemsLimit:
                    return Messages.ItemsLimit(
                        context: context,
                        data: data);
                case Types.JoeAccountCheck:
                    return Messages.JoeAccountCheck(
                        context: context,
                        data: data);
                case Types.LockedRecord:
                    return Messages.LockedRecord(
                        context: context,
                        data: data);
                case Types.LockedTable:
                    return Messages.LockedTable(
                        context: context,
                        data: data);
                case Types.LoginIdAlreadyUse:
                    return Messages.LoginIdAlreadyUse(
                        context: context,
                        data: data);
                case Types.MailAddressHasNotSet:
                    return Messages.MailAddressHasNotSet(
                        context: context,
                        data: data);
                case Types.NoLinks:
                    return Messages.NoLinks(
                        context: context,
                        data: data);
                case Types.NotFound:
                    return Messages.NotFound(
                        context: context,
                        data: data);
                case Types.NotLockedRecord:
                    return Messages.NotLockedRecord(
                        context: context,
                        data: data);
                case Types.NotMatchRegex:
                    return Messages.NotMatchRegex(
                        context: context,
                        data: data);
                case Types.NotRequiredColumn:
                    return Messages.NotRequiredColumn(
                        context: context,
                        data: data);
                case Types.OverLimitApi:
                    return Messages.OverLimitApi(
                        context: context,
                        data: data);
                case Types.OverLimitQuantity:
                    return Messages.OverLimitQuantity(
                        context: context,
                        data: data);
                case Types.OverLimitSize:
                    return Messages.OverLimitSize(
                        context: context,
                        data: data);
                case Types.OverLocalFolderLimitSize:
                    return Messages.OverLocalFolderLimitSize(
                        context: context,
                        data: data);
                case Types.OverLocalFolderTotalLimitSize:
                    return Messages.OverLocalFolderTotalLimitSize(
                        context: context,
                        data: data);
                case Types.OverTenantStorageSize:
                    return Messages.OverTenantStorageSize(
                        context: context,
                        data: data);
                case Types.OverTotalLimitSize:
                    return Messages.OverTotalLimitSize(
                        context: context,
                        data: data);
                case Types.ParameterSyntaxError:
                    return Messages.ParameterSyntaxError(
                        context: context,
                        data: data);
                case Types.PasswordNotChanged:
                    return Messages.PasswordNotChanged(
                        context: context,
                        data: data);
                case Types.PasswordPolicyViolation:
                    return Messages.PasswordPolicyViolation(
                        context: context,
                        data: data);
                case Types.PermissionNotSelfChange:
                    return Messages.PermissionNotSelfChange(
                        context: context,
                        data: data);
                case Types.RequireMailAddresses:
                    return Messages.RequireMailAddresses(
                        context: context,
                        data: data);
                case Types.RequireManagePermission:
                    return Messages.RequireManagePermission(
                        context: context,
                        data: data);
                case Types.RequireTo:
                    return Messages.RequireTo(
                        context: context,
                        data: data);
                case Types.Restricted:
                    return Messages.Restricted(
                        context: context,
                        data: data);
                case Types.SamlLoginFailed:
                    return Messages.SamlLoginFailed(
                        context: context,
                        data: data);
                case Types.SelectFile:
                    return Messages.SelectFile(
                        context: context,
                        data: data);
                case Types.SelectOne:
                    return Messages.SelectOne(
                        context: context,
                        data: data);
                case Types.SelectTargets:
                    return Messages.SelectTargets(
                        context: context,
                        data: data);
                case Types.SitesLimit:
                    return Messages.SitesLimit(
                        context: context,
                        data: data);
                case Types.TooLongText:
                    return Messages.TooLongText(
                        context: context,
                        data: data);
                case Types.TooManyCases:
                    return Messages.TooManyCases(
                        context: context,
                        data: data);
                case Types.TooManyColumnCases:
                    return Messages.TooManyColumnCases(
                        context: context,
                        data: data);
                case Types.TooManyRowCases:
                    return Messages.TooManyRowCases(
                        context: context,
                        data: data);
                case Types.Unauthorized:
                    return Messages.Unauthorized(
                        context: context,
                        data: data);
                case Types.UpdateConflicts:
                    return Messages.UpdateConflicts(
                        context: context,
                        data: data);
                case Types.UserDisabled:
                    return Messages.UserDisabled(
                        context: context,
                        data: data);
                case Types.UserLockout:
                    return Messages.UserLockout(
                        context: context,
                        data: data);
                case Types.UserNotSelfDelete:
                    return Messages.UserNotSelfDelete(
                        context: context,
                        data: data);
                case Types.UsersLimit:
                    return Messages.UsersLimit(
                        context: context,
                        data: data);
                default: return null;
            }
        }

        public static string MessageJson(this Types type, Context context, params string[] data)
        {
            return new ResponseCollection().Message(type.Message(
                context: context,
                data: data)).ToJson();
        }
    }
}
