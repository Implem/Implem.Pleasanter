using System;
using System.Collections.Generic;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Microsoft.AspNetCore.Http;

namespace Implem.Pleasanter.Libraries.Scim
{
    public partial class ScimDirectoryService
    {
        private static ScimServiceResult TryUserFilterWhere(
            Context context,
            string filter,
            out Rds.UsersWhereCollection where)
        {
            where = Rds.UsersWhere()
                .TenantId(context.TenantId)
                .ScimId(_operator: " is not null");
            var parseError = ParseEqFilters(filter, out var filters);
            if (parseError != null)
            {
                return parseError;
            }
            foreach (var (path, value) in filters)
            {
                switch (NormalizeFilterPath(path))
                {
                    case "id":
                        where.ScimId(value);
                        break;
                    case "externalid":
                        where.ScimExternalId(value);
                        break;
                    case "username":
                        where.LoginId(value);
                        break;
                    case "displayname":
                        where.Name(value);
                        break;
                    case "emails.value":
                        where.UserId_In(sub: Rds.SelectMailAddresses(
                            column: Rds.MailAddressesColumn().OwnerId(),
                            where: Rds.MailAddressesWhere()
                                .OwnerType("Users")
                                .MailAddress(value)));
                        break;
                    case "active":
                        where.Disabled(!BoolValue(value));
                        break;
                    case "urn:ietf:params:scim:schemas:extension:enterprise:2.0:user:employeenumber":
                    case "employeenumber":
                        where.UserCode(value);
                        break;
                    case "manager":
                    case "manager.value":
                    case "urn:ietf:params:scim:schemas:extension:enterprise:2.0:user:manager":
                    case "urn:ietf:params:scim:schemas:extension:enterprise:2.0:user:manager.value":
                        where.Manager_In(sub: Rds.SelectUsers(
                            column: Rds.UsersColumn().UserId(),
                            where: Rds.UsersWhere()
                                .TenantId(context.TenantId)
                                .ScimId(value)));
                        break;
                    default:
                        return InvalidFilter($"Unsupported filter attribute: {path}");
                }
            }
            return null;
        }

        private static ScimServiceResult TryGroupFilterWhere(
            Context context,
            string filter,
            out Rds.GroupsWhereCollection where)
        {
            where = Rds.GroupsWhere()
                .TenantId(context.TenantId)
                .ScimId(_operator: " is not null")
                .Disabled(false);
            var parseError = ParseEqFilters(filter, out var filters);
            if (parseError != null)
            {
                return parseError;
            }
            foreach (var (path, value) in filters)
            {
                switch (NormalizeFilterPath(path))
                {
                    case "id":
                        where.ScimId(value);
                        break;
                    case "externalid":
                        where.ScimExternalId(value);
                        break;
                    case "displayname":
                        where.GroupName(value);
                        break;
                    default:
                        return InvalidFilter($"Unsupported filter attribute: {path}");
                }
            }
            return null;
        }

        private static ScimServiceResult ParseEqFilters(
            string filter,
            out List<(string path, string value)> filters)
        {
            filters = [];
            if (filter.IsNullOrEmpty() || filter.Trim().Length == 0)
            {
                return null;
            }
            foreach (var part in SplitFilterAnd(filter))
            {
                var trimmed = part.Trim();
                if (trimmed.IsNullOrEmpty())
                {
                    return InvalidFilter("Invalid filter syntax");
                }
                var match = EqFilterRegex.Match(trimmed);
                if (!match.Success)
                {
                    return InvalidFilter("Invalid filter syntax");
                }
                filters.Add((match.Groups["path"].Value, match.Groups["value"].Value));
            }
            return null;
        }

        private static List<string> SplitFilterAnd(string filter)
        {
            var parts = new List<string>();
            var start = 0;
            var inQuote = false;
            var bracketDepth = 0;
            for (var i = 0; i < filter.Length; i++)
            {
                var ch = filter[i];
                if (ch == '"' && (i == 0 || filter[i - 1] != '\\'))
                {
                    inQuote = !inQuote;
                    continue;
                }
                if (!inQuote)
                {
                    if (ch == '[')
                    {
                        bracketDepth++;
                    }
                    else if (ch == ']' && bracketDepth > 0)
                    {
                        bracketDepth--;
                    }
                    else if (bracketDepth == 0 && IsLogicalAndAt(filter, i))
                    {
                        parts.Add(filter[start..i]);
                        i += 2;
                        start = i + 1;
                    }
                }
            }
            parts.Add(filter[start..]);
            return parts;
        }

        private static bool IsLogicalAndAt(string filter, int index)
        {
            return index > 0
                && index + 3 < filter.Length
                && char.IsWhiteSpace(filter[index - 1])
                && char.IsWhiteSpace(filter[index + 3])
                && string.Equals(
                    filter.Substring(index, 3),
                    "and",
                    StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeFilterPath(string path)
        {
            return NormalizePatchPath(path);
        }

        private static bool BoolValue(string value)
        {
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }

        private static ScimServiceResult InvalidFilter(string detail)
        {
            return Error(StatusCodes.Status400BadRequest, detail, "invalidFilter");
        }

    }
}