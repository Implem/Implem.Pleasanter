namespace Implem.Pleasanter.Libraries.General
{
    public static class Error
    {
        public enum Types
        {
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
    }
}
