using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class UserValidators
    {
        public static Error.Types OnCreating(SiteSettings ss, UserModel userModel)
        {
            if (!ss.CanCreate())
            {
                return Error.Types.HasNotPermission;
            }
            ss.SetColumnAccessControls(userModel.Mine());
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Users_LoginId":
                        if (!ss.GetColumn("LoginId").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_Disabled":
                        if (!ss.GetColumn("Disabled").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_UserCode":
                        if (!ss.GetColumn("UserCode").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_Password":
                        if (!ss.GetColumn("Password").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_LastName":
                        if (!ss.GetColumn("LastName").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_FirstName":
                        if (!ss.GetColumn("FirstName").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_Birthday":
                        if (!ss.GetColumn("Birthday").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_Gender":
                        if (!ss.GetColumn("Gender").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_Language":
                        if (!ss.GetColumn("Language").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_TimeZone":
                        if (!ss.GetColumn("TimeZone").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_DeptId":
                        if (!ss.GetColumn("DeptId").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_FirstAndLastNameOrder":
                        if (!ss.GetColumn("FirstAndLastNameOrder").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_LastLoginTime":
                        if (!ss.GetColumn("LastLoginTime").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_PasswordExpirationTime":
                        if (!ss.GetColumn("PasswordExpirationTime").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_PasswordChangeTime":
                        if (!ss.GetColumn("PasswordChangeTime").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_NumberOfLogins":
                        if (!ss.GetColumn("NumberOfLogins").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_NumberOfDenial":
                        if (!ss.GetColumn("NumberOfDenial").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_TenantManager":
                        if (!ss.GetColumn("TenantManager").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn("Comments").CanCreate)
                        {
                            return Error.Types.HasNotPermission;
                        }
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
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Users_LoginId":
                        if (userModel.LoginId_Updated &&
                            !ss.GetColumn("LoginId").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_Disabled":
                        if (userModel.Disabled_Updated &&
                            !ss.GetColumn("Disabled").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_UserCode":
                        if (userModel.UserCode_Updated &&
                            !ss.GetColumn("UserCode").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_Password":
                        if (userModel.Password_Updated &&
                            !ss.GetColumn("Password").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_LastName":
                        if (userModel.LastName_Updated &&
                            !ss.GetColumn("LastName").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_FirstName":
                        if (userModel.FirstName_Updated &&
                            !ss.GetColumn("FirstName").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_Birthday":
                        if (userModel.Birthday_Updated &&
                            !ss.GetColumn("Birthday").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_Gender":
                        if (userModel.Gender_Updated &&
                            !ss.GetColumn("Gender").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_Language":
                        if (userModel.Language_Updated &&
                            !ss.GetColumn("Language").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_TimeZone":
                        if (userModel.TimeZone_Updated &&
                            !ss.GetColumn("TimeZone").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_DeptId":
                        if (userModel.DeptId_Updated &&
                            !ss.GetColumn("DeptId").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_FirstAndLastNameOrder":
                        if (userModel.FirstAndLastNameOrder_Updated &&
                            !ss.GetColumn("FirstAndLastNameOrder").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_LastLoginTime":
                        if (userModel.LastLoginTime_Updated &&
                            !ss.GetColumn("LastLoginTime").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_PasswordExpirationTime":
                        if (userModel.PasswordExpirationTime_Updated &&
                            !ss.GetColumn("PasswordExpirationTime").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_PasswordChangeTime":
                        if (userModel.PasswordChangeTime_Updated &&
                            !ss.GetColumn("PasswordChangeTime").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_NumberOfLogins":
                        if (userModel.NumberOfLogins_Updated &&
                            !ss.GetColumn("NumberOfLogins").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_NumberOfDenial":
                        if (userModel.NumberOfDenial_Updated &&
                            !ss.GetColumn("NumberOfDenial").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Users_TenantManager":
                        if (userModel.TenantManager_Updated &&
                            !ss.GetColumn("TenantManager").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                    case "Comments":
                        if (!ss.GetColumn("Comments").CanUpdate)
                        {
                            return Error.Types.HasNotPermission;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(SiteSettings ss, UserModel userModel)
        {
            if (!ss.CanDelete())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnRestoring()
        {
            if (!Permissions.CanManageTenant())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnExporting(SiteSettings ss)
        {
            if (!ss.CanExport())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnPasswordChanging(UserModel userModel)
        {
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
            if (userModel.Password == userModel.ChangedPassword)
            {
                return Error.Types.PasswordNotChanged;
            }
            else if (!userModel.GetByCredentials(userModel.LoginId, userModel.Password))
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
    }
}
