using System;
using System.Collections.Generic;
using System.Linq;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Settings;

namespace Implem.Pleasanter.Libraries.Scim
{
    public partial class ScimDirectoryService
    {
        private static void SetUserDepartment(ScimUser user, string department)
        {
            user.EnterpriseUser ??= new ScimEnterpriseUser();
            user.EnterpriseUser.Department = department?.Trim();
        }

        private static string UserDepartment(ScimUser user)
        {
            return user?.EnterpriseUser?.Department?.Trim();
        }

        private static void SetUserEmployeeNumber(ScimUser user, string employeeNumber)
        {
            user.EnterpriseUser ??= new ScimEnterpriseUser();
            user.EnterpriseUser.EmployeeNumber = employeeNumber?.Trim();
        }

        private static string UserCode(ScimUser user)
        {
            return user?.EnterpriseUser?.EmployeeNumber?.Trim();
        }

        private static bool IsEnterpriseUserPath(string path)
        {
            return string.Equals(
                path,
                ScimSchemas.EnterpriseUser.ToLowerInvariant(),
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsDepartmentPath(string path)
        {
            return string.Equals(path, "department", StringComparison.OrdinalIgnoreCase)
                || string.Equals(
                    path,
                    $"{ScimSchemas.EnterpriseUser.ToLowerInvariant()}:department",
                    StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsEmployeeNumberPath(string path)
        {
            return string.Equals(path, "employeenumber", StringComparison.OrdinalIgnoreCase)
                || string.Equals(
                    path,
                    $"{ScimSchemas.EnterpriseUser.ToLowerInvariant()}:employeenumber",
                    StringComparison.OrdinalIgnoreCase);
        }

        private static string UserLoginId(ScimUser user)
        {
            return user?.UserName
                ?? user?.Emails?.FirstOrDefault(email => email.Primary == true)?.Value
                ?? user?.Emails?.FirstOrDefault()?.Value;
        }

        private static string UserDisplayName(ScimUser user)
        {
            var nameParts = new[]
            {
                user?.Name?.GivenName,
                user?.Name?.FamilyName
            }
                .Where(part => !part.IsNullOrEmpty())
                .Join(" ");
            return user?.DisplayName
                ?? user?.Name?.Formatted
                ?? (nameParts.IsNullOrEmpty() ? null : nameParts)
                ?? user?.UserName;
        }

        private static readonly Dictionary<string, string> LanguageAliases =
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "vi", "vn" }
            };

        private static string ResolveLanguage(ScimUser user)
        {
            if (user == null)
            {
                return null;
            }
            var candidate = user.PreferredLanguage.IsNullOrEmpty()
                ? user.Locale
                : user.PreferredLanguage;
            return MapLanguageCode(candidate);
        }

        private static string ToScimLanguage(string pleasanterLanguage)
        {
            if (pleasanterLanguage.IsNullOrEmpty())
            {
                return null;
            }
            var reverse = LanguageAliases.FirstOrDefault(kv =>
                kv.Value.Equals(pleasanterLanguage, StringComparison.OrdinalIgnoreCase));
            return reverse.Key ?? pleasanterLanguage;
        }

        private static string MapLanguageCode(string raw)
        {
            if (raw.IsNullOrEmpty())
            {
                return null;
            }
            var trimmed = raw.Trim();
            var supported = MultilingualLabelExportImport.SupportedLanguages;
            var exactMatch = supported.FirstOrDefault(lang =>
                lang.Equals(trimmed, StringComparison.OrdinalIgnoreCase));
            if (exactMatch != null)
            {
                return exactMatch;
            }
            if (LanguageAliases.TryGetValue(trimmed, out var aliased))
            {
                return aliased;
            }
            var separatorIndex = trimmed.IndexOf('-');
            if (separatorIndex > 0)
            {
                return MapLanguageCode(trimmed[..separatorIndex]);
            }
            return null;
        }
    }
}
