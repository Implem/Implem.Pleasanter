using System;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Models;

namespace Implem.Pleasanter.Libraries.Scim
{
    public static class ScimTokenValidator
    {
        private const int LastUsedTimeUpdateIntervalSeconds = 60;

        public class ValidationResult
        {
            public int TenantId { get; set; }
            public int UserId { get; set; }
            public string TokenPrefix { get; set; }
        }

        public static bool TryValidate(
            Context context,
            string authorizationHeader,
            out ValidationResult result)
        {
            const string prefix = "Bearer ";
            result = null;
            var token = authorizationHeader?.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) == true
                ? authorizationHeader[prefix.Length..].Trim()
                : null;
            if (token.IsNullOrEmpty())
            {
                return false;
            }
            return TryValidateStoredToken(context, token, out result);
        }

        private static bool TryValidateStoredToken(
            Context context,
            string token,
            out ValidationResult result)
        {
            result = null;
            try
            {
                var tokenHash = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
                var tokens = new ScimTokenCollection(
                    context: context,
                    column: Rds.ScimTokensColumn()
                            .ScimTokenId()
                            .TenantId()
                            .UserId()
                            .TokenPrefix()
                            .ExpiresTime()
                            .LastUsedTime(),
                    where: Rds.ScimTokensWhere()
                        .Disabled(false)
                        .TokenHash(tokenHash));
                var model = tokens.FirstOrDefault(tokenModel => !TokenExpired(tokenModel.ExpiresTime));
                if (model == null)
                {
                    return false;
                }
                result = new ValidationResult
                {
                    TenantId = model.TenantId,
                    UserId = model.UserId,
                    TokenPrefix = model.TokenPrefix
                };
                if (result.TenantId <= 0
                    || result.UserId <= 0)
                {
                    return false;
                }
                if (!UserAllowed(context, result))
                {
                    return false;
                }
                UpdateLastUsedTime(context, model);
                return true;
            }
            catch (DbException)
            {
                return false;
            }
        }

        private static bool UserAllowed(Context context, ValidationResult result)
        {
            var userModel = new UserCollection(
                context: context,
                ss: null,
                column: Rds.UsersColumn()
                    .LoginId()
                    .TenantManager()
                    .Disabled(),
                where: Rds.UsersWhere()
                    .TenantId(result.TenantId)
                    .UserId(result.UserId),
                top: 1)
                    .FirstOrDefault();
            if (userModel == null
                || userModel.Disabled)
            {
                return false;
            }
            return userModel.TenantManager
                || Permissions.PrivilegedUsers(userModel.LoginId);
        }

        private static bool TokenExpired(DateTime expiresTime)
        {
            return expiresTime.InRange()
                && expiresTime <= DateTime.Now;
        }

        private static void UpdateLastUsedTime(Context context, ScimTokenModel model)
        {
            var now = DateTime.Now;
            if (model.LastUsedTime.InRange()
                && model.LastUsedTime > now.AddSeconds(-LastUsedTimeUpdateIntervalSeconds))
            {
                return;
            }
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.UpdateScimTokens(
                    param: Rds.ScimTokensParam()
                        .LastUsedTime(now),
                    where: Rds.ScimTokensWhere()
                        .ScimTokenId(model.ScimTokenId)));
        }
    }
}
