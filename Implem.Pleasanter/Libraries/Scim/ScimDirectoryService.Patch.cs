using System;
using System.Collections.Generic;
using System.Linq;
using Implem.Libraries.Utilities;
using Newtonsoft.Json.Linq;

namespace Implem.Pleasanter.Libraries.Scim
{
    public partial class ScimDirectoryService
    {
        private static void ApplyUserObject(ScimUser user, JObject obj)
        {
            if (obj["userName"] != null) user.UserName = obj["userName"].ToString();
            if (obj["externalId"] != null) user.ExternalId = obj["externalId"].ToString();
            if (obj["displayName"] != null) user.DisplayName = obj["displayName"].ToString();
            if (obj["active"] != null) user.Active = obj["active"].ToObject<bool>();
            if (obj["name"] is JObject name)
            {
                user.Name ??= new ScimName();
                if (name["givenName"] != null) user.Name.GivenName = name["givenName"].ToString();
                if (name["familyName"] != null) user.Name.FamilyName = name["familyName"].ToString();
                if (name["formatted"] != null) user.Name.Formatted = name["formatted"].ToString();
            }
            if (obj["emails"] != null)
            {
                user.Emails = ParseEmails(obj["emails"]);
            }
            if (obj["preferredLanguage"] != null) user.PreferredLanguage = obj["preferredLanguage"].ToString();
            if (obj["locale"] != null) user.Locale = obj["locale"].ToString();
            if (obj["timezone"] != null) user.Timezone = obj["timezone"].ToString();
            if (obj[ScimSchemas.EnterpriseUser] is JObject enterpriseUser)
            {
                ApplyEnterpriseUserObject(user, enterpriseUser);
            }
            if (obj["department"] != null)
            {
                SetUserDepartment(user, obj["department"].ToString());
            }
            if (obj["employeeNumber"] != null)
            {
                SetUserEmployeeNumber(user, obj["employeeNumber"].ToString());
            }
            if (obj["manager"] != null)
            {
                SetUserManager(user, obj["manager"]);
            }
            ApplyExtendedAttributeObject(
                extensionData: ExtensionData(user),
                attributes: ScimExtendedAttributeUtilities.UserAttributes(),
                tableName: ScimExtendedAttributeUtilities.UsersTableName,
                obj: obj);
            ApplyUserFlatObject(user, obj);
        }

        private static void ApplyUserPatch(
            ScimUser user,
            string op,
            string path,
            JToken value)
        {
            if (op == "remove")
            {
                if (IsEnterpriseUserPath(path))
                {
                    SetUserDepartment(user, string.Empty);
                    SetUserEmployeeNumber(user, string.Empty);
                    ClearUserManager(user);
                    return;
                }
                if (IsDepartmentPath(path))
                {
                    SetUserDepartment(user, string.Empty);
                    return;
                }
                if (IsEmployeeNumberPath(path))
                {
                    SetUserEmployeeNumber(user, string.Empty);
                    return;
                }
                if (IsManagerPath(path))
                {
                    ClearUserManager(user);
                    return;
                }
                if (ApplyExtendedAttributePatch(
                    extensionData: ExtensionData(user),
                    attributes: ScimExtendedAttributeUtilities.UserAttributes(),
                    tableName: ScimExtendedAttributeUtilities.UsersTableName,
                    op: op,
                    path: path,
                    value: value))
                {
                    return;
                }
                switch (path)
                {
                    case "emails":
                    case "emails.value":
                        user.Emails = [];
                        break;
                    case "preferredlanguage":
                        user.PreferredLanguage = null;
                        break;
                    case "locale":
                        user.Locale = null;
                        break;
                    case "timezone":
                        user.Timezone = null;
                        break;
                }
                return;
            }
            if (IsEnterpriseUserPath(path)
                && value is JObject enterpriseUser)
            {
                ApplyEnterpriseUserObject(user, enterpriseUser);
                return;
            }
            if (IsDepartmentPath(path))
            {
                SetUserDepartment(user, value?.ToString());
                return;
            }
            if (IsEmployeeNumberPath(path))
            {
                SetUserEmployeeNumber(user, value?.ToString());
                return;
            }
            if (IsManagerPath(path))
            {
                SetUserManager(user, value);
                return;
            }
            if (ApplyExtendedAttributePatch(
                extensionData: ExtensionData(user),
                attributes: ScimExtendedAttributeUtilities.UserAttributes(),
                tableName: ScimExtendedAttributeUtilities.UsersTableName,
                op: op,
                path: path,
                value: value))
            {
                return;
            }
            switch (path)
            {
                case "username":
                    user.UserName = value?.ToString();
                    break;
                case "externalid":
                    user.ExternalId = value?.ToString();
                    break;
                case "displayname":
                    user.DisplayName = value?.ToString();
                    break;
                case "active":
                    user.Active = value?.ToObject<bool>();
                    break;
                case "name.givenname":
                    user.Name ??= new ScimName();
                    user.Name.GivenName = value?.ToString();
                    break;
                case "name.familyname":
                    user.Name ??= new ScimName();
                    user.Name.FamilyName = value?.ToString();
                    break;
                case "name.formatted":
                    user.Name ??= new ScimName();
                    user.Name.Formatted = value?.ToString();
                    break;
                case "emails":
                case "emails.value":
                    user.Emails = ParseEmails(value);
                    break;
                case "preferredlanguage":
                    user.PreferredLanguage = value?.ToString();
                    break;
                case "locale":
                    user.Locale = value?.ToString();
                    break;
                case "timezone":
                    user.Timezone = value?.ToString();
                    break;
            }
        }

        private static void ApplyEnterpriseUserObject(ScimUser user, JObject obj)
        {
            if (obj["department"] != null)
            {
                SetUserDepartment(user, obj["department"].ToString());
            }
            if (obj["employeeNumber"] != null)
            {
                SetUserEmployeeNumber(user, obj["employeeNumber"].ToString());
            }
            if (obj["manager"] != null)
            {
                SetUserManager(user, obj["manager"]);
            }
        }

        private static void ApplyUserFlatObject(ScimUser user, JObject obj)
        {
            foreach (var property in obj.Properties())
            {
                if (string.Equals(property.Name, "schemas", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(property.Name, "meta", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                var path = NormalizePatchPath(property.Name);
                if (property.Value is JObject || property.Value is JArray)
                {
                    if (IsManagerPath(path))
                    {
                        ApplyUserPatch(
                            user: user,
                            op: "replace",
                            path: path,
                            value: property.Value);
                    }
                    if (ApplyExtendedAttributePatch(
                        extensionData: ExtensionData(user),
                        attributes: ScimExtendedAttributeUtilities.UserAttributes(),
                        tableName: ScimExtendedAttributeUtilities.UsersTableName,
                        op: "replace",
                        path: path,
                        value: property.Value))
                    {
                        continue;
                    }
                    continue;
                }
                ApplyUserPatch(
                    user: user,
                    op: "replace",
                    path: path,
                    value: property.Value);
            }
        }

        private enum GroupMemberPatchOperation
        {
            Add,
            Remove,
            Clear
        }

        private sealed class GroupMemberPatch
        {
            public GroupMemberPatchOperation Operation { get; init; }
            public List<ScimMember> Members { get; init; } = [];
        }

        private static void ApplyGroupObject(ScimGroup group, JObject obj)
        {
            if (obj["displayName"] != null) group.DisplayName = obj["displayName"].ToString();
            if (obj["externalId"] != null) group.ExternalId = obj["externalId"].ToString();
            if (obj["members"] != null) group.Members = ParseMembers(obj["members"]);
            ApplyExtendedAttributeObject(
                extensionData: ExtensionData(group),
                attributes: ScimExtendedAttributeUtilities.GroupAttributes(),
                tableName: ScimExtendedAttributeUtilities.GroupsTableName,
                obj: obj);
            ApplyGroupFlatObject(group, obj);
        }

        private static void ApplyGroupFlatObject(ScimGroup group, JObject obj)
        {
            foreach (var property in obj.Properties())
            {
                if (string.Equals(property.Name, "schemas", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(property.Name, "meta", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(property.Name, "members", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                ApplyExtendedAttributePatch(
                    extensionData: ExtensionData(group),
                    attributes: ScimExtendedAttributeUtilities.GroupAttributes(),
                    tableName: ScimExtendedAttributeUtilities.GroupsTableName,
                    op: "replace",
                    path: NormalizePatchPath(property.Name),
                    value: property.Value);
            }
        }

        private static bool ApplyGroupPatch(
            ScimGroup group,
            string op,
            string normalizedPath,
            string originalPath,
            JToken value,
            List<GroupMemberPatch> memberPatches)
        {
            if (normalizedPath == "displayname")
            {
                group.DisplayName = value?.ToString();
                return false;
            }
            if (normalizedPath == "externalid")
            {
                group.ExternalId = value?.ToString();
                return false;
            }
            if (ApplyExtendedAttributePatch(
                extensionData: ExtensionData(group),
                attributes: ScimExtendedAttributeUtilities.GroupAttributes(),
                tableName: ScimExtendedAttributeUtilities.GroupsTableName,
                op: op,
                path: normalizedPath,
                value: value))
            {
                return false;
            }
            if (normalizedPath != "members")
            {
                return false;
            }
            group.Members ??= [];
            switch (op)
            {
                case "remove":
                    var removeValue = ExtractMemberValue(originalPath);
                    var removeMembers = removeValue.IsNullOrEmpty()
                        ? ParseMembers(value)
                        : [new ScimMember { Value = removeValue }];
                    var removeValues = removeMembers
                        .Select(member => member.Value)
                        .Where(value => !value.IsNullOrEmpty())
                        .ToList();
                    if (removeValues.Count == 0)
                    {
                        group.Members.Clear();
                        memberPatches.Add(new GroupMemberPatch
                        {
                            Operation = GroupMemberPatchOperation.Clear
                        });
                    }
                    else
                    {
                        group.Members.RemoveAll(member =>
                            removeValues.Any(removeValue =>
                                string.Equals(member.Value, removeValue, StringComparison.OrdinalIgnoreCase)));
                        memberPatches.Add(new GroupMemberPatch
                        {
                            Operation = GroupMemberPatchOperation.Remove,
                            Members = removeMembers
                        });
                    }
                    return false;
                case "add":
                    var addMembers = ParseMembers(value);
                    addMembers.ForEach(member =>
                    {
                        if (!group.Members.Any(existing =>
                            string.Equals(existing.Value, member.Value, StringComparison.OrdinalIgnoreCase)))
                        {
                            group.Members.Add(member);
                        }
                    });
                    memberPatches.Add(new GroupMemberPatch
                    {
                        Operation = GroupMemberPatchOperation.Add,
                        Members = addMembers
                    });
                    return false;
                case "replace":
                    group.Members = ParseMembers(value);
                    return true;
            }
            return false;
        }

        private static string NormalizePatchPath(string path)
        {
            if (path.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var normalized = path.Trim();
            var bracketIndex = normalized.IndexOf('[');
            if (bracketIndex >= 0)
            {
                var endIndex = normalized.IndexOf(']', bracketIndex);
                if (endIndex >= 0)
                {
                    normalized = normalized.Remove(bracketIndex, endIndex - bracketIndex + 1);
                }
            }
            return normalized.ToLowerInvariant();
        }

        private static string ExtractMemberValue(string path)
        {
            if (path.IsNullOrEmpty())
            {
                return null;
            }
            var match = MemberPathRegex.Match(path);
            return match.Success
                ? match.Groups["value"].Value
                : null;
        }

        private static List<ScimEmail> ParseEmails(JToken token)
        {
            if (token == null)
            {
                return [];
            }
            if (token.Type == JTokenType.String)
            {
                return
                [
                    new ScimEmail { Value = token.ToString(), Type = "work", Primary = true }
                ];
            }
            return token.Type == JTokenType.Array
                ? token.ToObject<List<ScimEmail>>() ?? []
                : [];
        }

        private static List<ScimMember> ParseMembers(JToken token)
        {
            if (token == null)
            {
                return [];
            }
            if (token.Type == JTokenType.String)
            {
                return [new ScimMember { Value = token.ToString() }];
            }
            if (token.Type == JTokenType.Object)
            {
                var member = token.ToObject<ScimMember>();
                return member == null ? [] : [member];
            }
            return token.Type == JTokenType.Array
                ? token.ToObject<List<ScimMember>>() ?? []
                : [];
        }


    }
}