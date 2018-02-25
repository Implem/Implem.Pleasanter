using Implem.Libraries.Utilities;
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
            Authentication,
            BadFormat,
            BadMailAddress,
            BadRequest,
            CanNotChangeInheritance,
            CanNotDisabled,
            CanNotInherit,
            CanNotLink,
            CanNotPerformed,
            CantSetAtTopOfSite,
            DefinitionNotFound,
            DeleteConflicts,
            Expired,
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
            InvalidRequest,
            ItemsLimit,
            LoginIdAlreadyUse,
            MailAddressHasNotSet,
            NoLinks,
            NotFound,
            NotRequiredColumn,
            OverLimitQuantity,
            OverLimitSize,
            OverTenantStorageSize,
            OverTotalLimitSize,
            ParameterSyntaxError,
            PasswordNotChanged,
            PermissionNotSelfChange,
            RequireMailAddresses,
            RequireTo,
            Restricted,
            SelectFile,
            SelectOne,
            SelectTargets,
            SitesLimit,
            TooManyCases,
            TooManyColumnCases,
            TooManyRowCases,
            Unauthorized,
            UpdateConflicts,
            UsersLimit
        }

        public static bool Has(this Types type)
        {
            return type != Types.None;
        }

        public static Message Message(this Types type, params string[] data)
        {
            switch (type)
            {
                case Types.AlreadyAdded:
                    return Messages.AlreadyAdded(data);
                case Types.AlreadyLinked:
                    return Messages.AlreadyLinked(data);
                case Types.ApplicationError:
                    return Messages.ApplicationError(data);
                case Types.Authentication:
                    return Messages.Authentication(data);
                case Types.BadFormat:
                    return Messages.BadFormat(data);
                case Types.BadMailAddress:
                    return Messages.BadMailAddress(data);
                case Types.BadRequest:
                    return Messages.BadRequest(data);
                case Types.CanNotChangeInheritance:
                    return Messages.CanNotChangeInheritance(data);
                case Types.CanNotDisabled:
                    return Messages.CanNotDisabled(data);
                case Types.CanNotInherit:
                    return Messages.CanNotInherit(data);
                case Types.CanNotLink:
                    return Messages.CanNotLink(data);
                case Types.CanNotPerformed:
                    return Messages.CanNotPerformed(data);
                case Types.CantSetAtTopOfSite:
                    return Messages.CantSetAtTopOfSite(data);
                case Types.DefinitionNotFound:
                    return Messages.DefinitionNotFound(data);
                case Types.DeleteConflicts:
                    return Messages.DeleteConflicts(data);
                case Types.Expired:
                    return Messages.Expired(data);
                case Types.ExternalMailAddress:
                    return Messages.ExternalMailAddress(data);
                case Types.FailedReadFile:
                    return Messages.FailedReadFile(data);
                case Types.FileNotFound:
                    return Messages.FileNotFound(data);
                case Types.HasNotPermission:
                    return Messages.HasNotPermission(data);
                case Types.ImportMax:
                    return Messages.ImportMax(data);
                case Types.IncorrectCurrentPassword:
                    return Messages.IncorrectCurrentPassword(data);
                case Types.IncorrectFileFormat:
                    return Messages.IncorrectFileFormat(data);
                case Types.IncorrectSiteDeleting:
                    return Messages.IncorrectSiteDeleting(data);
                case Types.InputMailAddress:
                    return Messages.InputMailAddress(data);
                case Types.InternalServerError:
                    return Messages.InternalServerError(data);
                case Types.InvalidCsvData:
                    return Messages.InvalidCsvData(data);
                case Types.InvalidFormula:
                    return Messages.InvalidFormula(data);
                case Types.InvalidRequest:
                    return Messages.InvalidRequest(data);
                case Types.ItemsLimit:
                    return Messages.ItemsLimit(data);
                case Types.LoginIdAlreadyUse:
                    return Messages.LoginIdAlreadyUse(data);
                case Types.MailAddressHasNotSet:
                    return Messages.MailAddressHasNotSet(data);
                case Types.NoLinks:
                    return Messages.NoLinks(data);
                case Types.NotFound:
                    return Messages.NotFound(data);
                case Types.NotRequiredColumn:
                    return Messages.NotRequiredColumn(data);
                case Types.OverLimitQuantity:
                    return Messages.OverLimitQuantity(data);
                case Types.OverLimitSize:
                    return Messages.OverLimitSize(data);
                case Types.OverTenantStorageSize:
                    return Messages.OverTenantStorageSize(data);
                case Types.OverTotalLimitSize:
                    return Messages.OverTotalLimitSize(data);
                case Types.ParameterSyntaxError:
                    return Messages.ParameterSyntaxError(data);
                case Types.PasswordNotChanged:
                    return Messages.PasswordNotChanged(data);
                case Types.PermissionNotSelfChange:
                    return Messages.PermissionNotSelfChange(data);
                case Types.RequireMailAddresses:
                    return Messages.RequireMailAddresses(data);
                case Types.RequireTo:
                    return Messages.RequireTo(data);
                case Types.Restricted:
                    return Messages.Restricted(data);
                case Types.SelectFile:
                    return Messages.SelectFile(data);
                case Types.SelectOne:
                    return Messages.SelectOne(data);
                case Types.SelectTargets:
                    return Messages.SelectTargets(data);
                case Types.SitesLimit:
                    return Messages.SitesLimit(data);
                case Types.TooManyCases:
                    return Messages.TooManyCases(data);
                case Types.TooManyColumnCases:
                    return Messages.TooManyColumnCases(data);
                case Types.TooManyRowCases:
                    return Messages.TooManyRowCases(data);
                case Types.Unauthorized:
                    return Messages.Unauthorized(data);
                case Types.UpdateConflicts:
                    return Messages.UpdateConflicts(data);
                case Types.UsersLimit:
                    return Messages.UsersLimit(data);
                default: return null;
            }
        }

        public static string MessageJson(this Types type, params string[] data)
        {
            return new ResponseCollection().Message(type.Message(data)).ToJson();
        }
    }
}
