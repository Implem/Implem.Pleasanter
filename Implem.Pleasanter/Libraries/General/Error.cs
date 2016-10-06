using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
namespace Implem.Pleasanter.Libraries.General
{
    public static class Error
    {
        public enum Types
        {
            None,
            InvalidRequest,
            Authentication,
            PasswordNotChanged,
            UpdateConflicts,
            DeleteConflicts,
            HasNotPermission,
            IncorrectCurrentPassword,
            PermissionNotSelfChange,
            DefinitionNotFound,
            CantSetAtTopOfSite,
            NotFound,
            RequireMailAddresses,
            RequireColumn,
            ExternalMailAddress,
            BadMailAddress,
            MailAddressHasNotSet,
            BadFormat,
            FileNotFound,
            NotRequiredColumn,
            InvalidCsvData,
            FailedReadFile,
            CanNotDisabled,
            AlreadyAdded,
            InvalidFormula
        }

        public static bool Has(this Types type)
        {
            return type != Types.None;
        }

        public static Message Message(this Types type)
        {
            switch (type)
            {
                case Types.InvalidRequest: return Messages.InvalidRequest();
                case Types.Authentication: return Messages.Authentication();
                case Types.PasswordNotChanged: return Messages.PasswordNotChanged();
                case Types.UpdateConflicts: return Messages.UpdateConflicts();
                case Types.DeleteConflicts: return Messages.DeleteConflicts();
                case Types.HasNotPermission: return Messages.HasNotPermission();
                case Types.IncorrectCurrentPassword: return Messages.IncorrectCurrentPassword();
                case Types.PermissionNotSelfChange: return Messages.PermissionNotSelfChange();
                case Types.DefinitionNotFound: return Messages.DefinitionNotFound();
                case Types.CantSetAtTopOfSite: return Messages.CantSetAtTopOfSite();
                case Types.NotFound: return Messages.NotFound();
                case Types.RequireMailAddresses: return Messages.RequireMailAddresses();
                case Types.RequireColumn: return Messages.RequireColumn();
                case Types.ExternalMailAddress: return Messages.ExternalMailAddress();
                case Types.BadMailAddress: return Messages.BadMailAddress();
                case Types.MailAddressHasNotSet: return Messages.MailAddressHasNotSet();
                case Types.BadFormat: return Messages.BadFormat();
                case Types.FileNotFound: return Messages.FileNotFound();
                case Types.NotRequiredColumn: return Messages.NotRequiredColumn();
                case Types.InvalidCsvData: return Messages.InvalidCsvData();
                case Types.FailedReadFile: return Messages.FailedReadFile();
                case Types.CanNotDisabled: return Messages.CanNotDisabled();
                case Types.AlreadyAdded: return Messages.AlreadyAdded();
                case Types.InvalidFormula: return Messages.InvalidFormula();
                default: return null;
            }
        }

        public static string MessageJson(this Types type)
        {
            return new ResponseCollection().Message(type.Message()).ToJson();
        }
    }
}
