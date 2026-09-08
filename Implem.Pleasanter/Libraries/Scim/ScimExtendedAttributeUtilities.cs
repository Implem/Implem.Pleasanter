using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using ParameterScimExtendedAttribute = Implem.ParameterAccessor.Parts.ScimExtendedAttribute;

namespace Implem.Pleasanter.Libraries.Scim
{
    internal static class ScimExtendedAttributeUtilities
    {
        internal const string UsersTableName = "Users";
        internal const string GroupsTableName = "Groups";

        private static readonly Regex AttributeNameRegex = new(
            pattern: @"^[A-Za-z][A-Za-z0-9_-]*$",
            options: RegexOptions.Compiled);

        private static readonly Regex ColumnNameRegex = new(
            pattern: @"^[A-Za-z0-9]+$",
            options: RegexOptions.Compiled);

        internal static IEnumerable<ParameterScimExtendedAttribute> UserAttributes()
        {
            return Attributes(
                attributes: Parameters.Scim?.ExtendedAttributes?.Users,
                tableName: UsersTableName);
        }

        internal static IEnumerable<ParameterScimExtendedAttribute> GroupAttributes()
        {
            return Attributes(
                attributes: Parameters.Scim?.ExtendedAttributes?.Groups,
                tableName: GroupsTableName);
        }

        internal static IEnumerable<string> Schemas(
            IEnumerable<ParameterScimExtendedAttribute> attributes,
            string tableName)
        {
            return attributes
                .Select(attribute => Schema(attribute, tableName))
                .Where(schema => !schema.IsNullOrEmpty())
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }

        internal static string Schema(ParameterScimExtendedAttribute attribute, string tableName)
        {
            var schema = attribute?.Schema?.Trim();
            return schema.IsNullOrEmpty()
                ? DefaultSchema(tableName)
                : schema;
        }

        internal static string Name(ParameterScimExtendedAttribute attribute)
        {
            return attribute?.Name?.Trim();
        }

        internal static string ColumnName(ParameterScimExtendedAttribute attribute, string tableName)
        {
            var columnName = attribute?.ColumnName?.Trim();
            if (columnName.IsNullOrEmpty())
            {
                return null;
            }
            var prefix = tableName + "_";
            if (columnName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return columnName[prefix.Length..];
            }
            return columnName.Contains('_')
                ? null
                : columnName;
        }

        private static IEnumerable<ParameterScimExtendedAttribute> Attributes(
            IEnumerable<ParameterScimExtendedAttribute> attributes,
            string tableName)
        {
            return attributes == null
                ? []
                : attributes.Where(attribute => IsValid(attribute, tableName));
        }

        private static string DefaultSchema(string tableName)
        {
            return tableName == GroupsTableName
                ? ScimSchemas.GroupExtendedAttributes
                : ScimSchemas.UserExtendedAttributes;
        }

        private static bool IsValid(ParameterScimExtendedAttribute attribute, string tableName)
        {
            var name = Name(attribute);
            return attribute != null
                && !name.IsNullOrEmpty()
                && AttributeNameRegex.IsMatch(name)
                && IsValidSchema(attribute, tableName)
                && IsSupportedColumn(attribute, tableName);
        }

        private static bool IsValidSchema(ParameterScimExtendedAttribute attribute, string tableName)
        {
            var schema = Schema(attribute, tableName);
            return !schema.IsNullOrEmpty()
                && !schema.Contains('/')
                && !string.Equals(schema, ScimSchemas.User, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(schema, ScimSchemas.Group, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(schema, ScimSchemas.EnterpriseUser, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSupportedColumn(ParameterScimExtendedAttribute attribute, string tableName)
        {
            var columnName = ColumnName(attribute, tableName);
            if (columnName.IsNullOrEmpty()
                || !ColumnNameRegex.IsMatch(columnName))
            {
                return false;
            }
            var type = Def.ExtendedColumnTypes.Get(columnName);
            return type == "Class"
                || type == "Description";
        }
    }
}
