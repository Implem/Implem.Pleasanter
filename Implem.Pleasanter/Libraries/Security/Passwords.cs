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
            var userModel = new UserModel(Forms.Data("Users_LoginId"));
            var error = userModel.ChangePasswordAtLogin();
            return error.Has()
                ? error.MessageJson()
                : userModel.Allow(Forms.Data("ReturnUrl"), atLogin: true);
        }

        public static string Reset(
            SiteSettings ss, Permissions.Types permissionType, int userId)
        {
            var userModel = new UserModel(SiteSettingsUtility.UsersSiteSettings(), userId);
            var invalid = UserValidators.OnUpdating(ss, permissionType, userModel);
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