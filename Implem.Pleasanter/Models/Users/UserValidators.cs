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
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Users_LoginId":
                        if (!ss.GetColumn("LoginId").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Disabled":
                        if (!ss.GetColumn("Disabled").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_UserCode":
                        if (!ss.GetColumn("UserCode").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Password":
                        if (!ss.GetColumn("Password").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordValidate":
                        if (!ss.GetColumn("PasswordValidate").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordDummy":
                        if (!ss.GetColumn("PasswordDummy").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_RememberMe":
                        if (!ss.GetColumn("RememberMe").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LastName":
                        if (!ss.GetColumn("LastName").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FirstName":
                        if (!ss.GetColumn("FirstName").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FullName1":
                        if (!ss.GetColumn("FullName1").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FullName2":
                        if (!ss.GetColumn("FullName2").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Birthday":
                        if (!ss.GetColumn("Birthday").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Gender":
                        if (!ss.GetColumn("Gender").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Language":
                        if (!ss.GetColumn("Language").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TimeZone":
                        if (!ss.GetColumn("TimeZone").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_DeptId":
                        if (!ss.GetColumn("DeptId").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FirstAndLastNameOrder":
                        if (!ss.GetColumn("FirstAndLastNameOrder").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LastLoginTime":
                        if (!ss.GetColumn("LastLoginTime").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordExpirationTime":
                        if (!ss.GetColumn("PasswordExpirationTime").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordChangeTime":
                        if (!ss.GetColumn("PasswordChangeTime").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_NumberOfLogins":
                        if (!ss.GetColumn("NumberOfLogins").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_NumberOfDenial":
                        if (!ss.GetColumn("NumberOfDenial").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TenantManager":
                        if (!ss.GetColumn("TenantManager").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_OldPassword":
                        if (!ss.GetColumn("OldPassword").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ChangedPassword":
                        if (!ss.GetColumn("ChangedPassword").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ChangedPasswordValidator":
                        if (!ss.GetColumn("ChangedPasswordValidator").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_AfterResetPassword":
                        if (!ss.GetColumn("AfterResetPassword").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_AfterResetPasswordValidator":
                        if (!ss.GetColumn("AfterResetPasswordValidator").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_DemoMailAddress":
                        if (!ss.GetColumn("DemoMailAddress").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_SessionGuid":
                        if (!ss.GetColumn("SessionGuid").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanCreate(ss))
                        {
                            return Error.Types.InvalidRequest;
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
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Users_LoginId":
                        if (!ss.GetColumn("LoginId").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Disabled":
                        if (!ss.GetColumn("Disabled").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_UserCode":
                        if (!ss.GetColumn("UserCode").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Password":
                        if (!ss.GetColumn("Password").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordValidate":
                        if (!ss.GetColumn("PasswordValidate").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordDummy":
                        if (!ss.GetColumn("PasswordDummy").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_RememberMe":
                        if (!ss.GetColumn("RememberMe").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LastName":
                        if (!ss.GetColumn("LastName").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FirstName":
                        if (!ss.GetColumn("FirstName").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FullName1":
                        if (!ss.GetColumn("FullName1").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FullName2":
                        if (!ss.GetColumn("FullName2").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Birthday":
                        if (!ss.GetColumn("Birthday").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Gender":
                        if (!ss.GetColumn("Gender").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Language":
                        if (!ss.GetColumn("Language").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TimeZone":
                        if (!ss.GetColumn("TimeZone").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_DeptId":
                        if (!ss.GetColumn("DeptId").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FirstAndLastNameOrder":
                        if (!ss.GetColumn("FirstAndLastNameOrder").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LastLoginTime":
                        if (!ss.GetColumn("LastLoginTime").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordExpirationTime":
                        if (!ss.GetColumn("PasswordExpirationTime").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordChangeTime":
                        if (!ss.GetColumn("PasswordChangeTime").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_NumberOfLogins":
                        if (!ss.GetColumn("NumberOfLogins").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_NumberOfDenial":
                        if (!ss.GetColumn("NumberOfDenial").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TenantManager":
                        if (!ss.GetColumn("TenantManager").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_OldPassword":
                        if (!ss.GetColumn("OldPassword").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ChangedPassword":
                        if (!ss.GetColumn("ChangedPassword").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ChangedPasswordValidator":
                        if (!ss.GetColumn("ChangedPasswordValidator").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_AfterResetPassword":
                        if (!ss.GetColumn("AfterResetPassword").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_AfterResetPasswordValidator":
                        if (!ss.GetColumn("AfterResetPasswordValidator").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_DemoMailAddress":
                        if (!ss.GetColumn("DemoMailAddress").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_SessionGuid":
                        if (!ss.GetColumn("SessionGuid").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanUpdate(ss))
                        {
                            return Error.Types.InvalidRequest;
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
            if (!Permissions.CanManageTenant() && !userModel.Self())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}
