using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class UserValidators
    {
        public static ErrorData OnEntry(Context context, SiteSettings ss, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (!Parameters.Service.ShowProfiles)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return context.HasPermission(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnReading(Context context, SiteSettings ss, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (!Parameters.Service.ShowProfiles)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return context.CanRead(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnEditing(
            Context context, SiteSettings ss, UserModel userModel, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (!Parameters.Service.ShowProfiles)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            switch (userModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        context.CanRead(ss: ss) &&
                        userModel.AccessStatus != Databases.AccessStatuses.NotFound
                            ? new ErrorData(type: Error.Types.None)
                            : new ErrorData(type: Error.Types.NotFound);
                case BaseModel.MethodTypes.New:
                    return context.CanCreate(ss: ss)
                        ? new ErrorData(type: Error.Types.None)
                        : new ErrorData(type: Error.Types.HasNotPermission);
                default:
                    return new ErrorData(type: Error.Types.NotFound);
            }
        }

        public static ErrorData OnCreating(
            Context context, SiteSettings ss, UserModel userModel, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (!Parameters.Service.ShowProfiles)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (!context.CanCreate(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            ss.SetColumnAccessControls(context: context, mine: userModel.Mine(context: context));
            foreach (var column in ss.Columns
                .Where(o => !o.CanCreate)
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "LoginId":
                        if (userModel.LoginId_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "GlobalId":
                        if (userModel.GlobalId_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Name":
                        if (userModel.Name_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "UserCode":
                        if (userModel.UserCode_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Password":
                        if (userModel.Password_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LastName":
                        if (userModel.LastName_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "FirstName":
                        if (userModel.FirstName_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Gender":
                        if (userModel.Gender_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Language":
                        if (userModel.Language_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "TimeZone":
                        if (userModel.TimeZone_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "DeptId":
                        if (userModel.DeptId_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "FirstAndLastNameOrder":
                        if (userModel.FirstAndLastNameOrder_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Body":
                        if (userModel.Body_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "NumberOfLogins":
                        if (userModel.NumberOfLogins_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "NumberOfDenial":
                        if (userModel.NumberOfDenial_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "TenantManager":
                        if (userModel.TenantManager_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Disabled":
                        if (userModel.Disabled_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Lockout":
                        if (userModel.Lockout_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LockoutCounter":
                        if (userModel.LockoutCounter_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ApiKey":
                        if (userModel.ApiKey_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LdapSearchRoot":
                        if (userModel.LdapSearchRoot_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Birthday":
                        if (userModel.Birthday_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LastLoginTime":
                        if (userModel.LastLoginTime_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "PasswordExpirationTime":
                        if (userModel.PasswordExpirationTime_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "PasswordChangeTime":
                        if (userModel.PasswordChangeTime_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "SynchronizedTime":
                        if (userModel.SynchronizedTime_Updated(context: context, column: column))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn(context: context, columnName: "Comments").CanUpdate)
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                if (userModel.Class_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Num":
                                if (userModel.Num_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Date":
                                if (userModel.Date_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Description":
                                if (userModel.Description_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Check":
                                if (userModel.Check_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Attachments":
                                if (userModel.Attachments_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                        }
                        break;
                }
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnUpdating(
            Context context, SiteSettings ss, UserModel userModel, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (context.Forms.Exists("Users_TenantManager")
                && userModel.Self(context: context))
            {
                return new ErrorData(type: Error.Types.PermissionNotSelfChange);
            }
            if (!context.CanUpdate(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            ss.SetColumnAccessControls(context: context, mine: userModel.Mine(context: context));
            foreach (var column in ss.Columns
                .Where(o => !o.CanUpdate)
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "LoginId":
                        if (userModel.LoginId_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "GlobalId":
                        if (userModel.GlobalId_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Name":
                        if (userModel.Name_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "UserCode":
                        if (userModel.UserCode_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Password":
                        if (userModel.Password_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LastName":
                        if (userModel.LastName_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "FirstName":
                        if (userModel.FirstName_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Birthday":
                        if (userModel.Birthday_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Gender":
                        if (userModel.Gender_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Language":
                        if (userModel.Language_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "TimeZone":
                        if (userModel.TimeZone_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "DeptId":
                        if (userModel.DeptId_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "FirstAndLastNameOrder":
                        if (userModel.FirstAndLastNameOrder_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Body":
                        if (userModel.Body_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LastLoginTime":
                        if (userModel.LastLoginTime_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "PasswordExpirationTime":
                        if (userModel.PasswordExpirationTime_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "PasswordChangeTime":
                        if (userModel.PasswordChangeTime_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "NumberOfLogins":
                        if (userModel.NumberOfLogins_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "NumberOfDenial":
                        if (userModel.NumberOfDenial_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "TenantManager":
                        if (userModel.TenantManager_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Disabled":
                        if (userModel.Disabled_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Lockout":
                        if (userModel.Lockout_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LockoutCounter":
                        if (userModel.LockoutCounter_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "ApiKey":
                        if (userModel.ApiKey_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "LdapSearchRoot":
                        if (userModel.LdapSearchRoot_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "SynchronizedTime":
                        if (userModel.SynchronizedTime_Updated(context: context))
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn(context: context, columnName: "Comments").CanUpdate)
                        {
                            return new ErrorData(type: Error.Types.HasNotPermission);
                        }
                        break;
                    default:
                        switch (Def.ExtendedColumnTypes.Get(column.Name))
                        {
                            case "Class":
                                if (userModel.Class_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Num":
                                if (userModel.Num_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Date":
                                if (userModel.Date_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Description":
                                if (userModel.Description_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Check":
                                if (userModel.Check_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                            case "Attachments":
                                if (userModel.Attachments_Updated(
                                    columnName: column.Name,
                                    context: context,
                                    column: column))
                                {
                                    return new ErrorData(type: Error.Types.HasNotPermission);
                                }
                                break;
                        }
                        break;
                }
            }
            return new ErrorData(type: Error.Types.None);
        }

        public static ErrorData OnDeleting(
            Context context, SiteSettings ss, UserModel userModel, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (!Parameters.Service.ShowProfiles)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return context.CanDelete(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnRestoring(Context context, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (!Parameters.Service.ShowProfiles)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return Permissions.CanManageTenant(context: context)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        public static ErrorData OnExporting(Context context, SiteSettings ss, bool api = false)
        {
            if (api && (context.ContractSettings.Api == false || !Parameters.Api.Enabled))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (!Parameters.Service.ShowProfiles)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return context.CanExport(ss: ss)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnEntry(Context context, SiteSettings ss)
        {
            return Permissions.CanManageTenant(context: context)
                ? new ErrorData(type: Error.Types.None)
                : new ErrorData(type: Error.Types.HasNotPermission);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnPasswordChanging(Context context, UserModel userModel)
        {
            if (!Parameters.Service.ShowProfiles)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (userModel.UserId == context.UserId)
            {
                if (userModel.OldPassword == userModel.ChangedPassword)
                {
                    return new ErrorData(type: Error.Types.PasswordNotChanged);
                }
                if (!userModel.GetByCredentials(
                    context: context,
                    loginId: userModel.LoginId,
                    password: userModel.OldPassword))
                {
                    return new ErrorData(type: Error.Types.IncorrectCurrentPassword);
                }
            }
            else
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnPasswordChangingAtLogin(
            Context context, UserModel userModel)
        {
            if (!Parameters.Service.ShowProfiles)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (userModel.Password == userModel.ChangedPassword)
            {
                return new ErrorData(type: Error.Types.PasswordNotChanged);
            }
            else if (!userModel.GetByCredentials(
                context: context,
                loginId: userModel.LoginId,
                password: userModel.Password,
                tenantId: context.Forms.Int("SelectedTenantId")))
            {
                return new ErrorData(type: Error.Types.IncorrectCurrentPassword);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnPasswordResetting(Context context)
        {
            if (!Parameters.Service.ShowProfiles)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (!Permissions.CanManageTenant(context: context))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnAddingMailAddress(
            Context context,
            UserModel userModel,
            string mailAddress,
            out string data)
        {
            var errorData = MailAddressValidators.BadMailAddress(
                context: context,
                addresses: mailAddress,
                data: out data);
            if (!Parameters.Service.ShowProfiles)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (errorData.Type.Has())
            {
                return errorData;
            }
            if (mailAddress.Trim() == string.Empty)
            {
                return new ErrorData(type: Error.Types.InputMailAddress);
            }
            if (!Permissions.CanManageTenant(context: context)
                && !userModel.Self(context: context))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnApiEditing(Context context, UserModel userModel)
        {
            if (context.ContractSettings.Api == false || !Parameters.Api.Enabled)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnApiCreating(Context context, UserModel userModel)
        {
            if (context.ContractSettings.Api == false || !Parameters.Api.Enabled)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnApiDeleting(Context context, UserModel userModel)
        {
            if (context.ContractSettings.Api == false || !Parameters.Api.Enabled)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnSwitchUser(Context context, UserModel userModel)
        {
            if (!Permissions.PrivilegedUsers(context.LoginId))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            if (userModel.Disabled)
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return new ErrorData(type: Error.Types.None);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnReturnSwitchUser(Context context)
        {
            if (!Permissions.PrivilegedUsers(context.LoginId))
            {
                return new ErrorData(type: Error.Types.InvalidRequest);
            }
            return new ErrorData(type: Error.Types.None);
        }
    }
}
