using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.Libraries.Scim
{
    public static class ScimSchemas
    {
        public const string User = "urn:ietf:params:scim:schemas:core:2.0:User";
        public const string Group = "urn:ietf:params:scim:schemas:core:2.0:Group";
        public const string EnterpriseUser = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User";
        public const string UserExtendedAttributes = "urn:pleasanter:params:scim:schemas:extension:custom:2.0:User";
        public const string GroupExtendedAttributes = "urn:pleasanter:params:scim:schemas:extension:custom:2.0:Group";
        public const string ListResponse = "urn:ietf:params:scim:api:messages:2.0:ListResponse";
        public const string PatchOp = "urn:ietf:params:scim:api:messages:2.0:PatchOp";
        public const string Error = "urn:ietf:params:scim:api:messages:2.0:Error";
    }

    public class ScimMeta
    {
        [JsonProperty("resourceType")]
        public string ResourceType { get; set; }

        [JsonProperty("created")]
        public string Created { get; set; }

        [JsonProperty("lastModified")]
        public string LastModified { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }
    }

    public class ScimName
    {
        [JsonProperty("formatted")]
        public string Formatted { get; set; }

        [JsonProperty("familyName")]
        public string FamilyName { get; set; }

        [JsonProperty("givenName")]
        public string GivenName { get; set; }
    }

    public class ScimEmail
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = "work";

        [JsonProperty("primary")]
        public bool? Primary { get; set; }
    }

    public class ScimEnterpriseUser
    {
        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("employeeNumber")]
        public string EmployeeNumber { get; set; }

        [JsonProperty("manager")]
        [JsonConverter(typeof(ScimManagerConverter))]
        public ScimManager Manager { get; set; }
    }

    public class ScimManager
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("$ref")]
        public string Ref { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }

    public class ScimManagerConverter : JsonConverter<ScimManager>
    {
        public override ScimManager ReadJson(
            JsonReader reader,
            Type objectType,
            ScimManager existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            return token.Type switch
            {
                JTokenType.Null => null,
                JTokenType.String => new ScimManager { Value = token.ToString() },
                JTokenType.Array => ReadObject(token.FirstOrDefault() as JObject),
                JTokenType.Object => ReadObject((JObject)token),
                _ => null,
            };
        }

        private static ScimManager ReadObject(JObject obj)
        {
            if (obj == null)
            {
                return null;
            }
            return new ScimManager
            {
                Value = obj["value"]?.ToString(),
                Ref = obj["$ref"]?.ToString(),
                DisplayName = obj["displayName"]?.ToString()
            };
        }

        public override void WriteJson(
            JsonWriter writer,
            ScimManager value,
            JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            var obj = new JObject();
            if (!string.IsNullOrEmpty(value.Value))
            {
                obj["value"] = value.Value;
            }
            if (!string.IsNullOrEmpty(value.Ref))
            {
                obj["$ref"] = value.Ref;
            }
            if (!string.IsNullOrEmpty(value.DisplayName))
            {
                obj["displayName"] = value.DisplayName;
            }
            obj.WriteTo(writer);
        }
    }

    public class ScimMember
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("display")]
        public string Display { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("$ref")]
        public string Ref { get; set; }
    }

    public class ScimUser
    {
        [JsonProperty("schemas")]
        public List<string> Schemas { get; set; } = [ScimSchemas.User];

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("name")]
        public ScimName Name { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("active")]
        public bool? Active { get; set; }

        [JsonProperty("emails")]
        public List<ScimEmail> Emails { get; set; }

        [JsonProperty("preferredLanguage")]
        public string PreferredLanguage { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty(ScimSchemas.EnterpriseUser)]
        public ScimEnterpriseUser EnterpriseUser { get; set; }

        [JsonProperty("meta")]
        public ScimMeta Meta { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> ExtensionData { get; set; } = new Dictionary<string, JToken>();
    }

    public class ScimGroup
    {
        [JsonProperty("schemas")]
        public List<string> Schemas { get; set; } = [ScimSchemas.Group];

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("members")]
        public List<ScimMember> Members { get; set; }

        [JsonProperty("meta")]
        public ScimMeta Meta { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> ExtensionData { get; set; } = new Dictionary<string, JToken>();
    }

    public class ScimListResponse
    {
        [JsonProperty("schemas")]
        public List<string> Schemas { get; set; } = [ScimSchemas.ListResponse];

        [JsonProperty("totalResults")]
        public int TotalResults { get; set; }

        [JsonProperty("Resources")]
        public object Resources { get; set; }

        [JsonProperty("startIndex")]
        public int StartIndex { get; set; }

        [JsonProperty("itemsPerPage")]
        public int ItemsPerPage { get; set; }
    }

    public class ScimPatchRequest
    {
        [JsonProperty("schemas")]
        public List<string> Schemas { get; set; }

        [JsonProperty("Operations")]
        public List<ScimPatchOperation> Operations { get; set; }
    }

    public class ScimPatchOperation
    {
        [JsonProperty("op")]
        public string Op { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("value")]
        public JToken Value { get; set; }
    }

    public class ScimError
    {
        [JsonProperty("schemas")]
        public List<string> Schemas { get; set; } = [ScimSchemas.Error];

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("scimType")]
        public string ScimType { get; set; }

        [JsonProperty("detail")]
        public string Detail { get; set; }
    }
}
