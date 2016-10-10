using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class UserValidators
    {
        public static Error.Types OnCreating(
            SiteSettings siteSettings, Permissions.Types permissionType, UserModel userModel)
        {
            if (!permissionType.CanEditTenant() && !userModel.Self())
            {
                return Error.Types.HasNotPermission;
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Users_TenantId":
                        if (!siteSettings.GetColumn("TenantId").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_UserId":
                        if (!siteSettings.GetColumn("UserId").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Ver":
                        if (!siteSettings.GetColumn("Ver").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LoginId":
                        if (!siteSettings.GetColumn("LoginId").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Disabled":
                        if (!siteSettings.GetColumn("Disabled").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_UserCode":
                        if (!siteSettings.GetColumn("UserCode").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Password":
                        if (!siteSettings.GetColumn("Password").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordValidate":
                        if (!siteSettings.GetColumn("PasswordValidate").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordDummy":
                        if (!siteSettings.GetColumn("PasswordDummy").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_RememberMe":
                        if (!siteSettings.GetColumn("RememberMe").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LastName":
                        if (!siteSettings.GetColumn("LastName").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FirstName":
                        if (!siteSettings.GetColumn("FirstName").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FullName1":
                        if (!siteSettings.GetColumn("FullName1").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FullName2":
                        if (!siteSettings.GetColumn("FullName2").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Birthday":
                        if (!siteSettings.GetColumn("Birthday").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Sex":
                        if (!siteSettings.GetColumn("Sex").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Language":
                        if (!siteSettings.GetColumn("Language").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TimeZone":
                        if (!siteSettings.GetColumn("TimeZone").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TimeZoneInfo":
                        if (!siteSettings.GetColumn("TimeZoneInfo").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_DeptId":
                        if (!siteSettings.GetColumn("DeptId").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Dept":
                        if (!siteSettings.GetColumn("Dept").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FirstAndLastNameOrder":
                        if (!siteSettings.GetColumn("FirstAndLastNameOrder").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Title":
                        if (!siteSettings.GetColumn("Title").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LastLoginTime":
                        if (!siteSettings.GetColumn("LastLoginTime").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordExpirationTime":
                        if (!siteSettings.GetColumn("PasswordExpirationTime").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordChangeTime":
                        if (!siteSettings.GetColumn("PasswordChangeTime").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_NumberOfLogins":
                        if (!siteSettings.GetColumn("NumberOfLogins").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_NumberOfDenial":
                        if (!siteSettings.GetColumn("NumberOfDenial").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TenantAdmin":
                        if (!siteSettings.GetColumn("TenantAdmin").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ServiceAdmin":
                        if (!siteSettings.GetColumn("ServiceAdmin").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Developer":
                        if (!siteSettings.GetColumn("Developer").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_OldPassword":
                        if (!siteSettings.GetColumn("OldPassword").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ChangedPassword":
                        if (!siteSettings.GetColumn("ChangedPassword").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ChangedPasswordValidator":
                        if (!siteSettings.GetColumn("ChangedPasswordValidator").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_AfterResetPassword":
                        if (!siteSettings.GetColumn("AfterResetPassword").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_AfterResetPasswordValidator":
                        if (!siteSettings.GetColumn("AfterResetPasswordValidator").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_MailAddresses":
                        if (!siteSettings.GetColumn("MailAddresses").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_DemoMailAddress":
                        if (!siteSettings.GetColumn("DemoMailAddress").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_SessionGuid":
                        if (!siteSettings.GetColumn("SessionGuid").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Comments":
                        if (!siteSettings.GetColumn("Comments").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Creator":
                        if (!siteSettings.GetColumn("Creator").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Updator":
                        if (!siteSettings.GetColumn("Updator").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_CreatedTime":
                        if (!siteSettings.GetColumn("CreatedTime").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_UpdatedTime":
                        if (!siteSettings.GetColumn("UpdatedTime").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_VerUp":
                        if (!siteSettings.GetColumn("VerUp").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Timestamp":
                        if (!siteSettings.GetColumn("Timestamp").CanCreate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(
            SiteSettings siteSettings, Permissions.Types permissionType, UserModel userModel)
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
                    case "Users_TenantId":
                        if (!siteSettings.GetColumn("TenantId").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_UserId":
                        if (!siteSettings.GetColumn("UserId").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Ver":
                        if (!siteSettings.GetColumn("Ver").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LoginId":
                        if (!siteSettings.GetColumn("LoginId").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Disabled":
                        if (!siteSettings.GetColumn("Disabled").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_UserCode":
                        if (!siteSettings.GetColumn("UserCode").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Password":
                        if (!siteSettings.GetColumn("Password").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordValidate":
                        if (!siteSettings.GetColumn("PasswordValidate").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordDummy":
                        if (!siteSettings.GetColumn("PasswordDummy").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_RememberMe":
                        if (!siteSettings.GetColumn("RememberMe").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LastName":
                        if (!siteSettings.GetColumn("LastName").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FirstName":
                        if (!siteSettings.GetColumn("FirstName").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FullName1":
                        if (!siteSettings.GetColumn("FullName1").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FullName2":
                        if (!siteSettings.GetColumn("FullName2").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Birthday":
                        if (!siteSettings.GetColumn("Birthday").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Sex":
                        if (!siteSettings.GetColumn("Sex").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Language":
                        if (!siteSettings.GetColumn("Language").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TimeZone":
                        if (!siteSettings.GetColumn("TimeZone").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TimeZoneInfo":
                        if (!siteSettings.GetColumn("TimeZoneInfo").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_DeptId":
                        if (!siteSettings.GetColumn("DeptId").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Dept":
                        if (!siteSettings.GetColumn("Dept").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_FirstAndLastNameOrder":
                        if (!siteSettings.GetColumn("FirstAndLastNameOrder").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Title":
                        if (!siteSettings.GetColumn("Title").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_LastLoginTime":
                        if (!siteSettings.GetColumn("LastLoginTime").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordExpirationTime":
                        if (!siteSettings.GetColumn("PasswordExpirationTime").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_PasswordChangeTime":
                        if (!siteSettings.GetColumn("PasswordChangeTime").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_NumberOfLogins":
                        if (!siteSettings.GetColumn("NumberOfLogins").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_NumberOfDenial":
                        if (!siteSettings.GetColumn("NumberOfDenial").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_TenantAdmin":
                        if (!siteSettings.GetColumn("TenantAdmin").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ServiceAdmin":
                        if (!siteSettings.GetColumn("ServiceAdmin").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Developer":
                        if (!siteSettings.GetColumn("Developer").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_OldPassword":
                        if (!siteSettings.GetColumn("OldPassword").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ChangedPassword":
                        if (!siteSettings.GetColumn("ChangedPassword").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_ChangedPasswordValidator":
                        if (!siteSettings.GetColumn("ChangedPasswordValidator").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_AfterResetPassword":
                        if (!siteSettings.GetColumn("AfterResetPassword").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_AfterResetPasswordValidator":
                        if (!siteSettings.GetColumn("AfterResetPasswordValidator").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_MailAddresses":
                        if (!siteSettings.GetColumn("MailAddresses").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_DemoMailAddress":
                        if (!siteSettings.GetColumn("DemoMailAddress").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_SessionGuid":
                        if (!siteSettings.GetColumn("SessionGuid").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Comments":
                        if (!siteSettings.GetColumn("Comments").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Creator":
                        if (!siteSettings.GetColumn("Creator").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Updator":
                        if (!siteSettings.GetColumn("Updator").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_CreatedTime":
                        if (!siteSettings.GetColumn("CreatedTime").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_UpdatedTime":
                        if (!siteSettings.GetColumn("UpdatedTime").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_VerUp":
                        if (!siteSettings.GetColumn("VerUp").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                    case "Users_Timestamp":
                        if (!siteSettings.GetColumn("Timestamp").CanUpdate(permissionType))
                        {
                            return Error.Types.InvalidRequest;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }

        public static Error.Types OnDeleting(
            SiteSettings siteSettings, Permissions.Types permissionType, UserModel userModel)
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
    }
}
