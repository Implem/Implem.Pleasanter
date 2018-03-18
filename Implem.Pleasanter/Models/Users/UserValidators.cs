using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class UserValidators
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnEntry(SiteSettings ss)
        {
            return Permissions.CanManageTenant()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnReading(SiteSettings ss)
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            return ss.CanRead()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnEditing(SiteSettings ss, UserModel userModel)
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            switch (userModel.MethodType)
            {
                case BaseModel.MethodTypes.Edit:
                    return
                        ss.CanRead()&&
                        userModel.AccessStatus != Databases.AccessStatuses.NotFound
                            ? Error.Types.None
                            : Error.Types.NotFound;        
                case BaseModel.MethodTypes.New:
                    return ss.CanCreate()
                        ? Error.Types.None
                        : Error.Types.HasNotPermission;
                default:
                    return Error.Types.NotFound;
            }
        }

        public static Error.Types OnCreating(SiteSettings ss, UserModel userModel)
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            if (!ss.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(userModel.Mine());
            foreach (var column in ss.Columns
                .Where(o => !o.CanCreate)
                .Where(o => !ss.FormulaTarget(o.ColumnName))
                .Where(o => !o.Linking))
            {
                switch (column.ColumnName)
                {
                    case "LoginId":
                        if (userModel.LoginId_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "GlobalId":
                        if (userModel.GlobalId_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Name":
                        if (userModel.Name_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "UserCode":
                        if (userModel.UserCode_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Password":
                        if (userModel.Password_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "LastName":
                        if (userModel.LastName_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "FirstName":
                        if (userModel.FirstName_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Gender":
                        if (userModel.Gender_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Language":
                        if (userModel.Language_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "TimeZone":
                        if (userModel.TimeZone_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "DeptId":
                        if (userModel.DeptId_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "FirstAndLastNameOrder":
                        if (userModel.FirstAndLastNameOrder_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Body":
                        if (userModel.Body_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumberOfLogins":
                        if (userModel.NumberOfLogins_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "NumberOfDenial":
                        if (userModel.NumberOfDenial_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "TenantManager":
                        if (userModel.TenantManager_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Disabled":
                        if (userModel.Disabled_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "ApiKey":
                        if (userModel.ApiKey_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Birthday":
                        if (userModel.Birthday_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "LastLoginTime":
                        if (userModel.LastLoginTime_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "PasswordExpirationTime":
                        if (userModel.PasswordExpirationTime_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "PasswordChangeTime":
                        if (userModel.PasswordChangeTime_Updated(column))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn("Comments").CanUpdate) return Error.Types.HasNotPermission;
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(SiteSettings ss, UserModel userModel)
        {
            if (Forms.Exists("Users_TenantManager") && userModel.Self())
            {
                return Error.Types.PermissionNotSelfChange;
            }
            if (!ss.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(userModel.Mine());
            foreach (var column in ss.Columns
                .Where(o => !o.CanUpdate)
                .Where(o => !ss.FormulaTarget(o.ColumnName)))
            {
                switch (column.ColumnName)
                {
                    case "LoginId":
                        if (userModel.LoginId_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "GlobalId":
                        if (userModel.GlobalId_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Name":
                        if (userModel.Name_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "UserCode":
                        if (userModel.UserCode_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Password":
                        if (userModel.Password_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "LastName":
                        if (userModel.LastName_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "FirstName":
                        if (userModel.FirstName_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Birthday":
                        if (userModel.Birthday_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Gender":
                        if (userModel.Gender_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Language":
                        if (userModel.Language_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "TimeZone":
                        if (userModel.TimeZone_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "DeptId":
                        if (userModel.DeptId_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "FirstAndLastNameOrder":
                        if (userModel.FirstAndLastNameOrder_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Body":
                        if (userModel.Body_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "LastLoginTime":
                        if (userModel.LastLoginTime_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "PasswordExpirationTime":
                        if (userModel.PasswordExpirationTime_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "PasswordChangeTime":
                        if (userModel.PasswordChangeTime_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumberOfLogins":
                        if (userModel.NumberOfLogins_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "NumberOfDenial":
                        if (userModel.NumberOfDenial_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "TenantManager":
                        if (userModel.TenantManager_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Disabled":
                        if (userModel.Disabled_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "ApiKey":
                        if (userModel.ApiKey_Updated()) return Error.Types.HasNotPermission;
                        break;
                    case "Comments":
                        if (!ss.GetColumn("Comments").CanUpdate) return Error.Types.HasNotPermission;
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(SiteSettings ss, UserModel userModel)
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            return ss.CanDelete()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnRestoring()
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            return Permissions.CanManageTenant()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        public static Error.Types OnExporting(SiteSettings ss)
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            return ss.CanExport()
                ? Error.Types.None
                : Error.Types.HasNotPermission;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnPasswordChanging(UserModel userModel)
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            if (userModel.UserId == Sessions.UserId())
            {
                if (userModel.OldPassword == userModel.ChangedPassword)
                {
                    return Error.Types.PasswordNotChanged;
                }
                if (!userModel.GetByCredentials(userModel.LoginId, userModel.OldPassword))
                {
                    return Error.Types.IncorrectCurrentPassword;
                }
            }
            else
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnPasswordChangingAtLogin(UserModel userModel)
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            if (userModel.Password == userModel.ChangedPassword)
            {
                return Error.Types.PasswordNotChanged;
            }
            else if (!userModel.GetByCredentials(
                userModel.LoginId, userModel.Password, Forms.Int("SelectedTenantId")))
            {
                return Error.Types.IncorrectCurrentPassword;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnPasswordResetting()
        {
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            if (!Permissions.CanManageTenant())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnAddingMailAddress(
            SiteSettings ss,
            UserModel userModel,
            string mailAddress,
            out string data)
        {
            var error = MailAddressValidators.BadMailAddress(mailAddress, out data);
            if (!DefinitionAccessor.Parameters.Service.ShowProfiles)
            {
                return Error.Types.InvalidRequest;
            }
            if (error.Has())
            {
                return error;
            }
            if (mailAddress.Trim() == string.Empty)
            {
                return Error.Types.InputMailAddress;
            }
            if (!Permissions.CanManageTenant() && !userModel.Self())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnApiEditing(UserModel userModel)
        {
            if (!Contract.Api())
            {
                return Error.Types.InvalidRequest;
            }
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Error.Types.InvalidRequest;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnApiCreating(UserModel userModel)
        {
            if (!Contract.Api())
            {
                return Error.Types.InvalidRequest;
            }
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Error.Types.InvalidRequest;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnApiDeleting(UserModel userModel)
        {
            if (!Contract.Api())
            {
                return Error.Types.InvalidRequest;
            }
            if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                return Error.Types.InvalidRequest;
            }
            return Error.Types.None;
        }
    }
}
