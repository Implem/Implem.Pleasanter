using System;
using Implem.Pleasanter.Libraries.Requests;

namespace Implem.Pleasanter.Libraries.Scim
{
    public partial class ScimDirectoryService
    {
        private static ScimMeta Meta(
            Context context,
            string resourceType,
            string id,
            DateTime? created,
            DateTime? updated)
        {
            return new ScimMeta
            {
                ResourceType = resourceType,
                Created = ToIso(created),
                LastModified = ToIso(updated),
                Location = ResourceLocation(context, resourceType == "User" ? "Users" : "Groups", id)
            };
        }

        private static string ResourceLocation(Context context, string resourceName, string id)
        {
            return $"{BaseLocation(context)}/{resourceName}/{id}";
        }

        private static string BaseLocation(Context context)
        {
            var absoluteUri = context.AbsoluteUri ?? string.Empty;
            var index = absoluteUri.IndexOf("/scim/v2", StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                return absoluteUri[..index] + "/scim/v2";
            }
            return "/scim/v2";
        }

        private static string ToIso(DateTime? value)
        {
            return value?.ToUniversalTime().ToString("o");
        }
    }
}
