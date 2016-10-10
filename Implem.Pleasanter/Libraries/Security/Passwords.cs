using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Security
{
    public static class Passwords
    {
        public static string Change(int id)
        {
            return new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                id,
                setByForm: true)
                    .ChangePassword();
        }

        public static string ChangeAtLogin()
        {
            return new UserModel(Forms.Data("Users_LoginId")).ChangePasswordAtLogin();
        }

        public static string Reset(
            SiteSettings siteSettings, Permissions.Types permissionType, int userId)
        {
            var userModel = new UserModel(SiteSettingsUtility.UsersSiteSettings(), userId);
            var invalid = UserValidators.OnUpdating(siteSettings, permissionType, userModel);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var error = userModel.ResetPassword();
            return error.Has()
                ? error.MessageJson()
                : new UsersResponseCollection(userModel)
                    .PasswordExpirationTime(userModel.PasswordExpirationTime.ToString())
                    .PasswordChangeTime(userModel.PasswordChangeTime.ToString())
                    .UpdatedTime(userModel.UpdatedTime.ToString())
                    .AfterResetPassword(string.Empty)
                    .AfterResetPasswordValidator(string.Empty)
                    .ClearFormData()
                    .CloseDialog()
                    .Message(Messages.PasswordResetCompleted()).ToJson();
        }

        public static string Default()
        {
            return "pleasanter";
        }
    }
}