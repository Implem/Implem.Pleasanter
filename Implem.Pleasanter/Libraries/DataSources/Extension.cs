using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class Extension
    {
        public static bool Authenticate(
            Context context,
            string loginId,
            string password,
            UserModel userModel)
        {
            if (userModel.GetByCredentials(
                context: context,
                loginId: loginId,
                password: password,
                tenantId: context.Forms.Int("SelectedTenantId")))
            {
                return true;
            }
            loginId = context.Forms.Data("Users_LoginId");
            password = context.Forms.Data("Users_Password");
            var enc = Encoding.GetEncoding("UTF-8");
            var requestUrl = Parameters.Authentication.ExtensionUrl;
            var tenantId = context.Forms.Get("SelectedTenantId") ?? context.TenantId.ToString();
            var key = Guid.NewGuid().ToString();
            SetLoginKey(
                context: context,
                loginId: loginId,
                key: key);
            var data = Encoding.Default.GetBytes(
                "ServiceId={0}&TenantId={1}&LoginId={2}&Key={3}&Password={4}".Params(
                    HttpUtility.UrlEncode(Parameters.Authentication.ServiceId),
                    HttpUtility.UrlEncode(tenantId),
                    HttpUtility.UrlEncode(loginId),
                    HttpUtility.UrlEncode(key),
                    HttpUtility.UrlEncode(password)));
            var req = WebRequest.Create(requestUrl);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            req.ContentLength = data.Length;
            using (Stream postStream = req.GetRequestStream())
            {
                postStream.Write(data, 0, data.Length);
            }
            using (var res = req.GetResponse())
            {
                var resStream = res.GetResponseStream();
                var sr = new StreamReader(resStream, enc);
                sr.Close();
            }
            var user = GetUser(
                context: context,
                loginId: loginId,
                key: key);
            if (user != null)
            {
                userModel.Get(
                    context: context,
                    ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                    where: Rds.UsersWhere()
                        .TenantId(user.TenantId)
                        .UserId(user.Id));
                return true;
            }
            return false;
        }

        private static void SetLoginKey(Context context, string loginId, string key)
        {
            Repository.ExecuteNonQuery(
                context: context,
                statements: new SqlStatement[]
                {
                    Rds.UpdateOrInsertLoginKeys(
                        where: Rds.LoginKeysWhere().LoginId(loginId),
                        param: Rds.LoginKeysParam()
                            .LoginId(loginId)
                            .Key(key)
                            .TenantNames(raw: "null")
                            .TenantId(raw: "null")
                            .UserId(raw: "null"))
                });
        }

        private static User GetUser(Context context, string loginId, string key)
        {
            var dataRow = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectLoginKeys(
                    column: Rds.LoginKeysColumn()
                        .TenantId()
                        .UserId(),
                    where: Rds.LoginKeysWhere()
                        .LoginId(loginId)
                        .Key(key)))
                            .AsEnumerable()
                            .FirstOrDefault(o => o.Int("UserId") > 0);
            if (dataRow != null)
            {
                return new User()
                {
                    TenantId = dataRow.Int("TenantId"),
                    Id = dataRow.Int("UserId")
                };
            }
            else
            {
                return null;
            }
        }
    }
}