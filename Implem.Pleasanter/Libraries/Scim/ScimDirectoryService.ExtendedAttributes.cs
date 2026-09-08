using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ParameterScimExtendedAttribute = Implem.ParameterAccessor.Parts.ScimExtendedAttribute;

namespace Implem.Pleasanter.Libraries.Scim
{
    public partial class ScimDirectoryService
    {
        private static void AddExtendedAttributeParams(
            Rds.UsersParamCollection param,
            IDictionary<string, JToken> extensionData,
            IEnumerable<ParameterScimExtendedAttribute> attributes,
            string tableName)
        {
            foreach (var data in ExtendedAttributeParamValues(extensionData, attributes, tableName))
            {
                param.Add(
                    columnBracket: $"\"{data.Key}\"",
                    name: data.Key,
                    value: data.Value,
                    sub: null,
                    raw: null);
            }
        }

        private static void AddExtendedAttributeParams(
            Rds.GroupsParamCollection param,
            IDictionary<string, JToken> extensionData,
            IEnumerable<ParameterScimExtendedAttribute> attributes,
            string tableName)
        {
            foreach (var data in ExtendedAttributeParamValues(extensionData, attributes, tableName))
            {
                param.Add(
                    columnBracket: $"\"{data.Key}\"",
                    name: data.Key,
                    value: data.Value,
                    sub: null,
                    raw: null);
            }
        }

        private static Dictionary<string, string> ExtendedAttributeParamValues(
            IDictionary<string, JToken> extensionData,
            IEnumerable<ParameterScimExtendedAttribute> attributes,
            string tableName)
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var attribute in attributes)
            {
                if (TryGetExtendedAttributeValue(
                    extensionData: extensionData,
                    attribute: attribute,
                    tableName: tableName,
                    value: out var value))
                {
                    data[ScimExtendedAttributeUtilities.ColumnName(attribute, tableName)] = value;
                }
            }
            return data;
        }

        private static void AddExtendedAttributeColumns(
            Rds.UsersColumnCollection columns,
            IEnumerable<ParameterScimExtendedAttribute> attributes,
            string tableName)
        {
            foreach (var columnName in ExtendedAttributeColumnNames(attributes, tableName))
            {
                columns.Add(
                    columnBracket: $"\"{columnName}\"",
                    columnName: columnName);
            }
        }

        private static void AddExtendedAttributeColumns(
            Rds.GroupsColumnCollection columns,
            IEnumerable<ParameterScimExtendedAttribute> attributes,
            string tableName)
        {
            foreach (var columnName in ExtendedAttributeColumnNames(attributes, tableName))
            {
                columns.Add(
                    columnBracket: $"\"{columnName}\"",
                    columnName: columnName);
            }
        }

        private static IEnumerable<string> ExtendedAttributeColumnNames(
            IEnumerable<ParameterScimExtendedAttribute> attributes,
            string tableName)
        {
            return attributes
                .Select(attribute => ScimExtendedAttributeUtilities.ColumnName(attribute, tableName))
                .Where(columnName => !columnName.IsNullOrEmpty())
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }

        private static void AddExtendedAttributeResponse(
            List<string> schemas,
            IDictionary<string, JToken> extensionData,
            BaseModel model,
            IEnumerable<ParameterScimExtendedAttribute> attributes,
            string tableName)
        {
            foreach (var attribute in attributes)
            {
                var value = ExtendedAttributeModelValue(
                    model: model,
                    columnName: ScimExtendedAttributeUtilities.ColumnName(attribute, tableName));
                if (value.IsNullOrEmpty())
                {
                    continue;
                }
                var schema = ScimExtendedAttributeUtilities.Schema(attribute, tableName);
                SetExtendedAttributeValue(
                    extensionData: extensionData,
                    schema: schema,
                    name: ScimExtendedAttributeUtilities.Name(attribute),
                    value: JValue.CreateString(value));
                if (!schemas.Any(existing => string.Equals(existing, schema, StringComparison.OrdinalIgnoreCase)))
                {
                    schemas.Add(schema);
                }
            }
        }

        private static string ExtendedAttributeModelValue(BaseModel model, string columnName)
        {
            return Def.ExtendedColumnTypes.Get(columnName) switch
            {
                "Class" => model.GetClass(columnName),
                "Description" => model.GetDescription(columnName),
                _ => null
            };
        }

        private static void ApplyExtendedAttributeObject(
            IDictionary<string, JToken> extensionData,
            IEnumerable<ParameterScimExtendedAttribute> attributes,
            string tableName,
            JObject obj)
        {
            foreach (var group in attributes.GroupBy(
                attribute => ScimExtendedAttributeUtilities.Schema(attribute, tableName),
                StringComparer.OrdinalIgnoreCase))
            {
                if (!obj.TryGetValue(group.Key, StringComparison.OrdinalIgnoreCase, out var token))
                {
                    continue;
                }
                if (token is not JObject extensionObj)
                {
                    if (token.Type == JTokenType.Null)
                    {
                        ClearExtendedAttributeValues(extensionData, group.Key, group);
                    }
                    continue;
                }
                foreach (var attribute in group)
                {
                    if (extensionObj.TryGetValue(
                        ScimExtendedAttributeUtilities.Name(attribute),
                        StringComparison.OrdinalIgnoreCase,
                        out var value))
                    {
                        SetExtendedAttributeValue(
                            extensionData: extensionData,
                            schema: group.Key,
                            name: ScimExtendedAttributeUtilities.Name(attribute),
                            value: value);
                    }
                }
            }
        }

        private static bool ApplyExtendedAttributePatch(
            IDictionary<string, JToken> extensionData,
            IEnumerable<ParameterScimExtendedAttribute> attributes,
            string tableName,
            string op,
            string path,
            JToken value)
        {
            foreach (var group in attributes.GroupBy(
                attribute => ScimExtendedAttributeUtilities.Schema(attribute, tableName),
                StringComparer.OrdinalIgnoreCase))
            {
                if (string.Equals(path, group.Key, StringComparison.OrdinalIgnoreCase))
                {
                    if (op == "remove")
                    {
                        ClearExtendedAttributeValues(extensionData, group.Key, group);
                        return true;
                    }
                    if (value is JObject obj)
                    {
                        ApplyExtendedAttributeObject(
                            extensionData: extensionData,
                            attributes: group,
                            tableName: tableName,
                            obj: new JObject { [group.Key] = obj });
                        return true;
                    }
                }
                foreach (var attribute in group)
                {
                    if (!string.Equals(
                        path,
                        $"{group.Key}:{ScimExtendedAttributeUtilities.Name(attribute)}",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    SetExtendedAttributeValue(
                        extensionData: extensionData,
                        schema: group.Key,
                        name: ScimExtendedAttributeUtilities.Name(attribute),
                        value: op == "remove"
                            ? JValue.CreateString(string.Empty)
                            : value);
                    return true;
                }
            }
            return false;
        }

        private static bool TryGetExtendedAttributeValue(
            IDictionary<string, JToken> extensionData,
            ParameterScimExtendedAttribute attribute,
            string tableName,
            out string value)
        {
            var schema = ScimExtendedAttributeUtilities.Schema(attribute, tableName);
            if (TryGetExtensionToken(extensionData, schema, out var schemaToken)
                && schemaToken is JObject obj
                && obj.TryGetValue(
                    ScimExtendedAttributeUtilities.Name(attribute),
                    StringComparison.OrdinalIgnoreCase,
                    out var attributeValue))
            {
                value = ExtendedAttributeStringValue(attributeValue);
                return true;
            }
            if (TryGetExtensionToken(
                extensionData,
                $"{schema}:{ScimExtendedAttributeUtilities.Name(attribute)}",
                out attributeValue))
            {
                value = ExtendedAttributeStringValue(attributeValue);
                return true;
            }
            value = null;
            return false;
        }

        private static void ClearExtendedAttributeValues(
            IDictionary<string, JToken> extensionData,
            string schema,
            IEnumerable<ParameterScimExtendedAttribute> attributes)
        {
            foreach (var attribute in attributes)
            {
                SetExtendedAttributeValue(
                    extensionData: extensionData,
                    schema: schema,
                    name: ScimExtendedAttributeUtilities.Name(attribute),
                    value: JValue.CreateString(string.Empty));
            }
        }

        private static void SetExtendedAttributeValue(
            IDictionary<string, JToken> extensionData,
            string schema,
            string name,
            JToken value)
        {
            if (!TryGetExtensionToken(extensionData, schema, out var token)
                || token is not JObject obj)
            {
                obj = [];
                extensionData[schema] = obj;
            }
            obj[name] = ExtendedAttributeStringValue(value);
        }

        private static bool TryGetExtensionToken(
            IDictionary<string, JToken> extensionData,
            string key,
            out JToken value)
        {
            foreach (var data in extensionData)
            {
                if (string.Equals(data.Key, key, StringComparison.OrdinalIgnoreCase))
                {
                    value = data.Value;
                    return true;
                }
            }
            value = null;
            return false;
        }

        private static string ExtendedAttributeStringValue(JToken value)
        {
            return value == null || value.Type == JTokenType.Null
                ? string.Empty
                : value.Type == JTokenType.String
                    ? value.ToString()
                    : value.ToString(Formatting.None);
        }

        private static IDictionary<string, JToken> ExtensionData(ScimUser user)
        {
            return user.ExtensionData ??= new Dictionary<string, JToken>();
        }

        private static IDictionary<string, JToken> ExtensionData(ScimGroup group)
        {
            return group.ExtensionData ??= new Dictionary<string, JToken>();
        }

    }
}