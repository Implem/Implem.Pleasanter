using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class UserValidators
    {
        public static Error.Types OnCreating(
            SiteSettings ss, Permissions.Types pt, UserModel userModel)
        {
            if (!pt.CanEditTenant() && !userModel.Self())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Users_LoginId":
                        if (!ss.GetColumn("LoginId").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Disabled":
                        if (!ss.GetColumn("Disabled").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_UserCode":
                        if (!ss.GetColumn("UserCode").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Password":
                        if (!ss.GetColumn("Password").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordValidate":
                        if (!ss.GetColumn("PasswordValidate").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordDummy":
                        if (!ss.GetColumn("PasswordDummy").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_RememberMe":
                        if (!ss.GetColumn("RememberMe").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LastName":
                        if (!ss.GetColumn("LastName").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FirstName":
                        if (!ss.GetColumn("FirstName").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FullName1":
                        if (!ss.GetColumn("FullName1").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FullName2":
                        if (!ss.GetColumn("FullName2").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Birthday":
                        if (!ss.GetColumn("Birthday").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Gender":
                        if (!ss.GetColumn("Gender").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Language":
                        if (!ss.GetColumn("Language").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TimeZone":
                        if (!ss.GetColumn("TimeZone").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_DeptId":
                        if (!ss.GetColumn("DeptId").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FirstAndLastNameOrder":
                        if (!ss.GetColumn("FirstAndLastNameOrder").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LastLoginTime":
                        if (!ss.GetColumn("LastLoginTime").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordExpirationTime":
                        if (!ss.GetColumn("PasswordExpirationTime").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordChangeTime":
                        if (!ss.GetColumn("PasswordChangeTime").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_NumberOfLogins":
                        if (!ss.GetColumn("NumberOfLogins").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_NumberOfDenial":
                        if (!ss.GetColumn("NumberOfDenial").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TenantAdmin":
                        if (!ss.GetColumn("TenantAdmin").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_OldPassword":
                        if (!ss.GetColumn("OldPassword").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ChangedPassword":
                        if (!ss.GetColumn("ChangedPassword").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ChangedPasswordValidator":
                        if (!ss.GetColumn("ChangedPasswordValidator").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_AfterResetPassword":
                        if (!ss.GetColumn("AfterResetPassword").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_AfterResetPasswordValidator":
                        if (!ss.GetColumn("AfterResetPasswordValidator").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_DemoMailAddress":
                        if (!ss.GetColumn("DemoMailAddress").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_SessionGuid":
                        if (!ss.GetColumn("SessionGuid").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanCreate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(
            SiteSettings ss, Permissions.Types pt, UserModel userModel)
        {
            if (Forms.Exists("Users_TenantAdmin") && userModel.Self())
            {
                return Error.Types.PermissionNotSelfChange;
            }
            if (!pt.CanEditTenant() && !userModel.Self())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Users_LoginId":
                        if (!ss.GetColumn("LoginId").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Disabled":
                        if (!ss.GetColumn("Disabled").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_UserCode":
                        if (!ss.GetColumn("UserCode").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Password":
                        if (!ss.GetColumn("Password").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordValidate":
                        if (!ss.GetColumn("PasswordValidate").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordDummy":
                        if (!ss.GetColumn("PasswordDummy").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_RememberMe":
                        if (!ss.GetColumn("RememberMe").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LastName":
                        if (!ss.GetColumn("LastName").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FirstName":
                        if (!ss.GetColumn("FirstName").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FullName1":
                        if (!ss.GetColumn("FullName1").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FullName2":
                        if (!ss.GetColumn("FullName2").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Birthday":
                        if (!ss.GetColumn("Birthday").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Gender":
                        if (!ss.GetColumn("Gender").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Language":
                        if (!ss.GetColumn("Language").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TimeZone":
                        if (!ss.GetColumn("TimeZone").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_DeptId":
                        if (!ss.GetColumn("DeptId").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FirstAndLastNameOrder":
                        if (!ss.GetColumn("FirstAndLastNameOrder").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LastLoginTime":
                        if (!ss.GetColumn("LastLoginTime").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordExpirationTime":
                        if (!ss.GetColumn("PasswordExpirationTime").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordChangeTime":
                        if (!ss.GetColumn("PasswordChangeTime").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_NumberOfLogins":
                        if (!ss.GetColumn("NumberOfLogins").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_NumberOfDenial":
                        if (!ss.GetColumn("NumberOfDenial").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TenantAdmin":
                        if (!ss.GetColumn("TenantAdmin").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_OldPassword":
                        if (!ss.GetColumn("OldPassword").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ChangedPassword":
                        if (!ss.GetColumn("ChangedPassword").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ChangedPasswordValidator":
                        if (!ss.GetColumn("ChangedPasswordValidator").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_AfterResetPassword":
                        if (!ss.GetColumn("AfterResetPassword").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_AfterResetPasswordValidator":
                        if (!ss.GetColumn("AfterResetPasswordValidator").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_DemoMailAddress":
                        if (!ss.GetColumn("DemoMailAddress").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_SessionGuid":
                        if (!ss.GetColumn("SessionGuid").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Timestamp":
                        if (!ss.GetColumn("Timestamp").CanUpdate(pt))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(
            SiteSettings ss, Permissions.Types pt, UserModel userModel)
        {
            if (!pt.CanEditTenant())
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
            Permissions.Types pt,
            UserModel userModel,
            string mailAddress,
            out string data)
        {
            var error = MailAddressValidators.BadMailAddress(mailAddress, out data);
            if (error.Has())
            {
                return error;
            }
            if (!pt.CanEditTenant() && !userModel.Self())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}
