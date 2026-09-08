using System;
using System.Linq;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Newtonsoft.Json.Linq;

namespace Implem.Pleasanter.Libraries.Scim
{
    public partial class ScimDirectoryService
    {
        private static bool IsManagerPath(string path)
        {
            var enterprise = ScimSchemas.EnterpriseUser.ToLowerInvariant();
            return string.Equals(path, "manager", StringComparison.OrdinalIgnoreCase)
                || string.Equals(path, "manager.value", StringComparison.OrdinalIgnoreCase)
                || string.Equals(path, $"{enterprise}:manager", StringComparison.OrdinalIgnoreCase)
                || string.Equals(path, $"{enterprise}:manager.value", StringComparison.OrdinalIgnoreCase);
        }

        private static void SetUserManager(ScimUser user, JToken token)
        {
            user.EnterpriseUser ??= new ScimEnterpriseUser();
            if (token == null || token.Type == JTokenType.Null)
            {
                user.EnterpriseUser.Manager = new ScimManager();
                return;
            }
            if (token is JArray array)
            {
                if (!array.Any())
                {
                    user.EnterpriseUser.Manager = new ScimManager();
                    return;
                }
                token = array.First;
            }
            if (token.Type == JTokenType.String)
            {
                user.EnterpriseUser.Manager = new ScimManager { Value = token.ToString() };
                return;
            }
            if (token is JObject obj)
            {
                user.EnterpriseUser.Manager = new ScimManager
                {
                    Value = obj["value"]?.ToString(),
                    Ref = obj["$ref"]?.ToString(),
                    DisplayName = obj["displayName"]?.ToString()
                };
            }
        }

        private static void ClearUserManager(ScimUser user)
        {
            user.EnterpriseUser ??= new ScimEnterpriseUser();
            user.EnterpriseUser.Manager = new ScimManager();
        }

        private sealed class ResolvedManager
        {
            public bool Specified { get; init; }
            public bool Clear { get; init; }
            public int? UserId { get; init; }
        }

        private static ResolvedManager ResolveManager(
            Context context,
            ScimUser user,
            string scimId,
            int existingUserId)
        {
            var manager = user?.EnterpriseUser?.Manager;
            if (manager == null)
            {
                return new ResolvedManager { Specified = false };
            }
            var value = manager.Value;
            if (value.IsNullOrEmpty())
            {
                value = ManagerIdFromRef(manager.Ref);
            }
            if (value.IsNullOrEmpty())
            {
                if (!manager.DisplayName.IsNullOrEmpty())
                {
                    return new ResolvedManager { Specified = false };
                }
                return new ResolvedManager { Specified = true, Clear = true };
            }
            var managerModel = FindUserModelByScimId(context, value)
                ?? FindUserModelByExternalId(context, value);
            if (managerModel == null)
            {
                return new ResolvedManager { Specified = false };
            }
            if (managerModel.UserId == existingUserId
                || string.Equals(managerModel.ScimId, scimId, StringComparison.OrdinalIgnoreCase))
            {
                return new ResolvedManager { Specified = false };
            }
            return new ResolvedManager
            {
                Specified = true,
                Clear = false,
                UserId = managerModel.UserId
            };
        }

        private static ScimManager ManagerReference(Context context, int userId)
        {
            if (userId <= 0)
            {
                return null;
            }
            var managerModel = new UserCollection(
                context: context,
                ss: null,
                column: Rds.UsersColumn()
                    .UserId()
                    .Name()
                    .ScimId(),
                where: Rds.UsersWhere()
                    .TenantId(context.TenantId)
                    .UserId(sub: Rds.SelectUsers(
                        column: Rds.UsersColumn().Manager(),
                        where: Rds.UsersWhere()
                            .TenantId(context.TenantId)
                            .UserId(userId))),
                top: 1)
                    .FirstOrDefault();
            if (managerModel == null || managerModel.ScimId.IsNullOrEmpty())
            {
                return null;
            }
            return new ScimManager
            {
                Value = managerModel.ScimId,
                Ref = ResourceLocation(context, "Users", managerModel.ScimId),
                DisplayName = managerModel.Name
            };
        }

        private static string ManagerIdFromRef(string reference)
        {
            if (reference.IsNullOrEmpty())
            {
                return null;
            }
            var trimmed = reference.TrimEnd('/');
            var index = trimmed.LastIndexOf('/');
            return index >= 0 && index < trimmed.Length - 1
                ? trimmed[(index + 1)..]
                : null;
        }
    }
}
