using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class UserValidators
    {
        public static Error.Types OnCreating(
            SiteSettings ss, Permissions.Types permissionType, UserModel userModel)
        {
            if (!permissionType.CanEditTenant() && !userModel.Self())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Users_LoginId":
                        if (!ss.GetColumn("LoginId").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Disabled":
                        if (!ss.GetColumn("Disabled").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_UserCode":
                        if (!ss.GetColumn("UserCode").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Password":
                        if (!ss.GetColumn("Password").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordValidate":
                        if (!ss.GetColumn("PasswordValidate").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordDummy":
                        if (!ss.GetColumn("PasswordDummy").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_RememberMe":
                        if (!ss.GetColumn("RememberMe").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LastName":
                        if (!ss.GetColumn("LastName").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FirstName":
                        if (!ss.GetColumn("FirstName").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Birthday":
                        if (!ss.GetColumn("Birthday").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Sex":
                        if (!ss.GetColumn("Sex").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Language":
                        if (!ss.GetColumn("Language").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TimeZone":
                        if (!ss.GetColumn("TimeZone").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_DeptId":
                        if (!ss.GetColumn("DeptId").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FirstAndLastNameOrder":
                        if (!ss.GetColumn("FirstAndLastNameOrder").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LastLoginTime":
                        if (!ss.GetColumn("LastLoginTime").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordExpirationTime":
                        if (!ss.GetColumn("PasswordExpirationTime").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordChangeTime":
                        if (!ss.GetColumn("PasswordChangeTime").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_NumberOfLogins":
                        if (!ss.GetColumn("NumberOfLogins").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_NumberOfDenial":
                        if (!ss.GetColumn("NumberOfDenial").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TenantAdmin":
                        if (!ss.GetColumn("TenantAdmin").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_OldPassword":
                        if (!ss.GetColumn("OldPassword").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ChangedPassword":
                        if (!ss.GetColumn("ChangedPassword").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ChangedPasswordValidator":
                        if (!ss.GetColumn("ChangedPasswordValidator").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_AfterResetPassword":
                        if (!ss.GetColumn("AfterResetPassword").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_AfterResetPasswordValidator":
                        if (!ss.GetColumn("AfterResetPasswordValidator").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_DemoMailAddress":
                        if (!ss.GetColumn("DemoMailAddress").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_SessionGuid":
                        if (!ss.GetColumn("SessionGuid").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(
            SiteSettings ss, Permissions.Types permissionType, UserModel userModel)
        {
            if (Forms.Exists("Users_TenantAdmin") && userModel.Self())
            {
                return Error.Types.PermissionNotSelfChange;
            }
            if (!permissionType.CanEditTenant() && !userModel.Self())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Users_LoginId":
                        if (!ss.GetColumn("LoginId").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Disabled":
                        if (!ss.GetColumn("Disabled").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_UserCode":
                        if (!ss.GetColumn("UserCode").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Password":
                        if (!ss.GetColumn("Password").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordValidate":
                        if (!ss.GetColumn("PasswordValidate").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordDummy":
                        if (!ss.GetColumn("PasswordDummy").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_RememberMe":
                        if (!ss.GetColumn("RememberMe").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LastName":
                        if (!ss.GetColumn("LastName").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FirstName":
                        if (!ss.GetColumn("FirstName").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Birthday":
                        if (!ss.GetColumn("Birthday").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Sex":
                        if (!ss.GetColumn("Sex").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Language":
                        if (!ss.GetColumn("Language").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TimeZone":
                        if (!ss.GetColumn("TimeZone").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_DeptId":
                        if (!ss.GetColumn("DeptId").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FirstAndLastNameOrder":
                        if (!ss.GetColumn("FirstAndLastNameOrder").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LastLoginTime":
                        if (!ss.GetColumn("LastLoginTime").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordExpirationTime":
                        if (!ss.GetColumn("PasswordExpirationTime").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordChangeTime":
                        if (!ss.GetColumn("PasswordChangeTime").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_NumberOfLogins":
                        if (!ss.GetColumn("NumberOfLogins").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_NumberOfDenial":
                        if (!ss.GetColumn("NumberOfDenial").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TenantAdmin":
                        if (!ss.GetColumn("TenantAdmin").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_OldPassword":
                        if (!ss.GetColumn("OldPassword").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ChangedPassword":
                        if (!ss.GetColumn("ChangedPassword").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ChangedPasswordValidator":
                        if (!ss.GetColumn("ChangedPasswordValidator").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_AfterResetPassword":
                        if (!ss.GetColumn("AfterResetPassword").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_AfterResetPasswordValidator":
                        if (!ss.GetColumn("AfterResetPasswordValidator").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_DemoMailAddress":
                        if (!ss.GetColumn("DemoMailAddress").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_SessionGuid":
                        if (!ss.GetColumn("SessionGuid").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(
            SiteSettings ss, Permissions.Types permissionType, UserModel userModel)
        {
            if (!permissionType.CanEditTenant())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnRestoring()
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnAddingMailAddress(
            Permissions.Types permissionType,
            UserModel userModel,
            string mailAddress,
            out string data)
        {
            var error = MailAddressValidators.BadMailAddress(mailAddress, out data);
            if (error.Has())
            {
                return error;
            }
            if (!permissionType.CanEditTenant() && !userModel.Self())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}
