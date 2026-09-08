using Implem.Libraries.Utilities;
using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class AiProviderUtilities
    {
        public const string ConnectionSettingMask = "********";

        private class ConnectionSettingKeyDefinition
        {
            public string Key;
            public bool Secret;
        }

        private static List<string> ProviderTypeList()
        {
            return new List<string>
            {
                AiProvider.Types.MarkdownFile,
                AiProvider.Types.Dify,
                AiProvider.Types.OpenAi
            };
        }

        public static Dictionary<string, string> ProviderTypes(Context context)
        {
            return ProviderTypeList().ToDictionary(
                providerType => providerType,
                providerType => ProviderTypeLabel(
                    context: context,
                    providerType: providerType));
        }

        private static string ProviderTypeLabel(Context context, string providerType)
        {
            switch (providerType)
            {
                case AiProvider.Types.MarkdownFile:
                    return Displays.MarkdownFile(context: context);
                case AiProvider.Types.OpenAi:
                    return "OpenAI";
                default:
                    return providerType;
            }
        }

        private static List<ConnectionSettingKeyDefinition> ConnectionSettingKeyDefinitions(
            string providerType)
        {
            switch (providerType)
            {
                case AiProvider.Types.Dify:
                    return new List<ConnectionSettingKeyDefinition>
                    {
                        new() { Key = "Endpoint" },
                        new() { Key = "DatasetId" },
                        new() { Key = "ApiKey", Secret = true }
                    };
                case AiProvider.Types.OpenAi:
                    return new List<ConnectionSettingKeyDefinition>
                    {
                        new() { Key = "Endpoint" },
                        new() { Key = "VectorStoreId" },
                        new() { Key = "ApiKey", Secret = true }
                    };
                default:
                    return new List<ConnectionSettingKeyDefinition>();
            }
        }

        private static List<string> SecretConnectionSettingKeys()
        {
            return ProviderTypeList()
                .SelectMany(providerType => ConnectionSettingKeyDefinitions(
                    providerType: providerType))
                .Where(definition => definition.Secret)
                .Select(definition => definition.Key)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static string ConnectionSettingTemplate(string providerType)
        {
            var keys = ConnectionSettingKeys(providerType: providerType);
            if (keys.Count == 0)
            {
                return string.Empty;
            }
            var template = new JObject();
            foreach (var key in keys)
            {
                template.Add(
                    key,
                    key == "Endpoint"
                        ? ConnectionSettingEndpoint(providerType: providerType)
                        : string.Empty);
            }
            return SerializeConnectionSetting(connectionSetting: template);
        }

        public static string MaskedConnectionSetting(AiProvider aiProvider)
        {
            if (aiProvider == null || aiProvider.ConnectionSetting.IsNullOrEmpty())
            {
                return aiProvider?.ConnectionSetting;
            }
            try
            {
                var connectionSetting = JToken.Parse(aiProvider.ConnectionSetting) as JObject;
                if (connectionSetting == null)
                {
                    return ConnectionSettingTemplate(providerType: aiProvider.ProviderType);
                }
                MaskConnectionSetting(
                    connectionSetting: connectionSetting,
                    secretKeys: SecretConnectionSettingKeys());
                return SerializeConnectionSetting(connectionSetting: connectionSetting);
            }
            catch (JsonReaderException)
            {
                return ConnectionSettingTemplate(providerType: aiProvider.ProviderType);
            }
        }

        public static string ResolveConnectionSetting(
            string providerType,
            string submitted,
            AiProvider stored)
        {
            if (submitted.IsNullOrEmpty())
            {
                return submitted;
            }
            JObject connectionSetting;
            try
            {
                connectionSetting = JToken.Parse(submitted) as JObject;
            }
            catch (JsonReaderException)
            {
                return submitted;
            }
            if (connectionSetting == null)
            {
                return submitted;
            }
            var storedConnectionSetting = StoredConnectionSetting(
                providerType: providerType,
                stored: stored);
            var secretKeys = SecretConnectionSettingKeys();
            ResolveConnectionSettingValues(
                connectionSetting: connectionSetting,
                storedConnectionSetting: storedConnectionSetting,
                secretKeys: secretKeys);
            EnsureTopLevelSecretConnectionSettingValues(
                connectionSetting: connectionSetting,
                storedConnectionSetting: storedConnectionSetting,
                providerType: providerType);
            return SerializeConnectionSetting(connectionSetting: connectionSetting);
        }

        private static JToken StoredConnectionSetting(string providerType, AiProvider stored)
        {
            if (stored == null
                || stored.ProviderType != providerType
                || stored.ConnectionSetting.IsNullOrEmpty())
            {
                return null;
            }
            try
            {
                return JToken.Parse(stored.ConnectionSetting);
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        private static void MaskConnectionSetting(
            JToken connectionSetting,
            List<string> secretKeys)
        {
            if (connectionSetting is JObject connectionSettingObject)
            {
                foreach (var property in connectionSettingObject.Properties())
                {
                    if (IsSecretConnectionSettingKey(
                        key: property.Name,
                        secretKeys: secretKeys)
                        && IsEmptyConnectionSettingValue(value: property.Value) == false)
                    {
                        property.Value = ConnectionSettingMask;
                    }
                    else
                    {
                        MaskConnectionSetting(
                            connectionSetting: property.Value,
                            secretKeys: secretKeys);
                    }
                }
            }
            else if (connectionSetting is JArray connectionSettingArray)
            {
                foreach (var item in connectionSettingArray)
                {
                    MaskConnectionSetting(
                        connectionSetting: item,
                        secretKeys: secretKeys);
                }
            }
        }

        private static void ResolveConnectionSettingValues(
            JToken connectionSetting,
            JToken storedConnectionSetting,
            List<string> secretKeys)
        {
            if (connectionSetting is JObject connectionSettingObject)
            {
                var storedConnectionSettingObject = storedConnectionSetting as JObject;
                foreach (var property in connectionSettingObject.Properties())
                {
                    var isSecret = IsSecretConnectionSettingKey(
                        key: property.Name,
                        secretKeys: secretKeys);
                    var storedProperty = StoredConnectionSettingProperty(
                        connectionSetting: storedConnectionSettingObject,
                        key: property.Name,
                        caseInsensitive: isSecret);
                    if (isSecret
                        && (IsEmptyConnectionSettingValue(value: property.Value)
                            || IsConnectionSettingMask(value: property.Value)))
                    {
                        property.Value = StoredConnectionSettingStringValue(
                            value: storedProperty?.Value);
                    }
                    else if (isSecret == false)
                    {
                        ResolveConnectionSettingValues(
                            connectionSetting: property.Value,
                            storedConnectionSetting: storedProperty?.Value,
                            secretKeys: secretKeys);
                    }
                }
            }
            else if (connectionSetting is JArray connectionSettingArray)
            {
                var storedConnectionSettingArray = storedConnectionSetting as JArray;
                for (var index = 0; index < connectionSettingArray.Count; index++)
                {
                    var storedValue = storedConnectionSettingArray != null
                        && index < storedConnectionSettingArray.Count
                        ? storedConnectionSettingArray[index]
                        : null;
                    ResolveConnectionSettingValues(
                        connectionSetting: connectionSettingArray[index],
                        storedConnectionSetting: storedValue,
                        secretKeys: secretKeys);
                }
            }
        }

        private static void EnsureTopLevelSecretConnectionSettingValues(
            JObject connectionSetting,
            JToken storedConnectionSetting,
            string providerType)
        {
            var storedConnectionSettingObject = storedConnectionSetting as JObject;
            foreach (var definition in ConnectionSettingKeyDefinitions(
                providerType: providerType).Where(definition => definition.Secret))
            {
                var property = StoredConnectionSettingProperty(
                    connectionSetting: connectionSetting,
                    key: definition.Key,
                    caseInsensitive: true);
                if (property == null)
                {
                    var storedProperty = StoredConnectionSettingProperty(
                        connectionSetting: storedConnectionSettingObject,
                        key: definition.Key,
                        caseInsensitive: true);
                    connectionSetting.Add(
                        definition.Key,
                        StoredConnectionSettingStringValue(value: storedProperty?.Value));
                }
            }
        }

        private static JProperty StoredConnectionSettingProperty(
            JObject connectionSetting,
            string key,
            bool caseInsensitive)
        {
            if (connectionSetting == null)
            {
                return null;
            }
            var exactMatch = connectionSetting.Properties().FirstOrDefault(property =>
                property.Name == key);
            if (exactMatch != null || caseInsensitive == false)
            {
                return exactMatch;
            }
            var caseInsensitiveMatches = connectionSetting
                .Properties()
                .Where(property => string.Equals(
                    property.Name,
                    key,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
            return caseInsensitiveMatches.Count == 1
                ? caseInsensitiveMatches[0]
                : null;
        }

        private static string StoredConnectionSettingStringValue(JToken value)
        {
            if (value?.Type != JTokenType.String)
            {
                return string.Empty;
            }
            return value.Value<string>() ?? string.Empty;
        }

        private static bool IsSecretConnectionSettingKey(
            string key,
            List<string> secretKeys)
        {
            return secretKeys.Any(secretKey => string.Equals(
                secretKey,
                key,
                StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsEmptyConnectionSettingValue(JToken value)
        {
            return value == null
                || value.Type == JTokenType.Null
                || value.Type == JTokenType.String && value.Value<string>().IsNullOrEmpty();
        }

        private static bool IsConnectionSettingMask(JToken value)
        {
            return value?.Type == JTokenType.String
                && value.Value<string>() == ConnectionSettingMask;
        }

        private static string SerializeConnectionSetting(JObject connectionSetting)
        {
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            })
            {
                connectionSetting.WriteTo(jsonWriter);
                return stringWriter.ToString().Replace(Environment.NewLine, "\n");
            }
        }

        public static List<string> ConnectionSettingKeys(string providerType)
        {
            return ConnectionSettingKeyDefinitions(providerType: providerType)
                .Select(definition => definition.Key)
                .ToList();
        }

        public static List<string> ConnectionSettingDisplayKeys(string providerType)
        {
            return ConnectionSettingKeyDefinitions(providerType: providerType)
                .Where(definition => definition.Secret == false)
                .Select(definition => definition.Key)
                .ToList();
        }

        public static string ConnectionSettingText(AiProvider aiProvider)
        {
            return ConnectionSettingKeyDefinitions(providerType: aiProvider?.ProviderType)
                .Select(definition => ConnectionSettingDisplayValue(
                    aiProvider: aiProvider,
                    definition: definition))
                .Where(value => value.IsNullOrEmpty() == false)
                .Join(" / ");
        }

        private static string ConnectionSettingDisplayValue(
            AiProvider aiProvider,
            ConnectionSettingKeyDefinition definition)
        {
            var value = ConnectionSettingValue(
                aiProvider: aiProvider,
                key: definition.Key);
            if (value.IsNullOrEmpty())
            {
                return string.Empty;
            }
            return definition.Secret
                ? ConnectionSettingMask
                : value;
        }

        public static bool RequireConnectionSetting(string providerType)
        {
            return ConnectionSettingKeyDefinitions(providerType: providerType).Any();
        }

        public static string ConnectionSettingRequiredTypes()
        {
            return ProviderTypeList()
                .Where(providerType => RequireConnectionSetting(providerType: providerType))
                .Join();
        }

        public static string ConnectionSettingValue(AiProvider aiProvider, string key)
        {
            if (RequireConnectionSetting(providerType: aiProvider?.ProviderType) == false)
            {
                return null;
            }
            if (aiProvider.ConnectionSetting.IsNullOrEmpty())
            {
                return null;
            }
            try
            {
                var json = JToken.Parse(aiProvider.ConnectionSetting) as JObject;
                if (json == null)
                {
                    return null;
                }
                var valueToken = json
                    ?.Property(key)
                    ?.Value;
                if (valueToken != null
                    && valueToken.Type != JTokenType.String
                    && valueToken.Type != JTokenType.Null)
                {
                    return null;
                }
                var value = valueToken?.Value<string>();
                if (key == "Endpoint" && value.IsNullOrEmpty())
                {
                    return ConnectionSettingEndpoint(providerType: aiProvider.ProviderType);
                }
                return value;
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        public static string ConnectionSettingEndpoint(string providerType)
        {
            var providerEndpoints = Parameters.AiConnect?.ProviderEndpoints;
            if (providerEndpoints == null
                || providerEndpoints.TryGetValue(providerType, out var endpoint) == false)
            {
                return string.Empty;
            }
            return endpoint ?? string.Empty;
        }
    }
}
