using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System;
using System.Diagnostics;

namespace Implem.Pleasanter.MCP.Infrastructure
{
    public static class McpApiKeyUserResolver
    {
        public static bool TryResolve(
            Context context,
            string apiKey,
            out int tenantId,
            out int userId)
        {
            tenantId = 0;
            userId = 0;

            if (string.IsNullOrEmpty(apiKey))
            {
                return false;
            }

            try
            {
                var userModel = new UserModel().Get(
                    context: context,
                    ss: null,
                    where: Rds.UsersWhere()
                        .ApiKey(apiKey)
                        .Disabled(false)
                        .Lockout(false));

                if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
                {
                    return false;
                }

                tenantId = userModel.TenantId;
                userId = userModel.UserId;
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                    $"[MCP] McpApiKeyUserResolver.TryResolve failed: {ex.Message}");
                return false;
            }
        }
    }
}
