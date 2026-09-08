using System;
using System.Collections.Generic;
using System.Linq;
using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.Scim;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Implem.Pleasanter.Controllers.Scim
{
    [CheckScimContextAttributes]
    [AllowAnonymous]
    [ApiController]
    [Route("scim/v2")]
    public class ScimMetadataController : ScimBaseController
    {
        private const string ServiceProviderConfigSchema =
            "urn:ietf:params:scim:schemas:core:2.0:ServiceProviderConfig";
        private const string ResourceTypeSchema =
            "urn:ietf:params:scim:schemas:core:2.0:ResourceType";
        private const string SchemaSchema =
            "urn:ietf:params:scim:schemas:core:2.0:Schema";
        private const string ListResponseSchema =
            "urn:ietf:params:scim:api:messages:2.0:ListResponse";

        [HttpGet("ServiceProviderConfig")]
        public ContentResult ServiceProviderConfig()
        {
            return WithSysLog(() => ScimResponse.Content(new JObject
            {
                ["schemas"] = new JArray(ServiceProviderConfigSchema),
                ["documentationUri"] =
                    "https://learn.microsoft.com/en-us/entra/identity/app-provisioning/use-scim-to-provision-users-and-groups",
                ["patch"] = new JObject { ["supported"] = true },
                ["bulk"] = new JObject
                {
                    ["supported"] = false,
                    ["maxOperations"] = 0,
                    ["maxPayloadSize"] = 0
                },
                ["filter"] = new JObject
                {
                    ["supported"] = true,
                    ["maxResults"] = ScimDirectoryService.MaxCount
                },
                ["changePassword"] = new JObject { ["supported"] = false },
                ["sort"] = new JObject { ["supported"] = false },
                ["etag"] = new JObject { ["supported"] = false },
                ["authenticationSchemes"] = new JArray
                {
                    new JObject
                    {
                        ["name"] = "OAuth Bearer Token",
                        ["description"] = "Bearer token",
                        ["type"] = "oauthbearertoken",
                        ["primary"] = true
                    }
                },
                ["meta"] = Meta("ServiceProviderConfig", "/ServiceProviderConfig")
            }));
        }

        [HttpGet("ResourceTypes")]
        public ContentResult ResourceTypes()
        {
            return WithSysLog(() => ScimResponse.Content(ListResponse(UserResourceType(), GroupResourceType())));
        }

        [HttpGet("ResourceTypes/{id}")]
        public ContentResult ResourceType(string id)
        {
            var resourceType = ResourceTypeResource(id);
            return WithSysLog(() => resourceType == null
                ? ScimResponse.Error(404, "ResourceType not found")
                : ScimResponse.Content(resourceType));
        }

        [HttpGet("Schemas")]
        public ContentResult Schemas()
        {
            var schemas = new List<JObject>
            {
                UserSchema(),
                GroupSchema(),
                EnterpriseUserSchema()
            };
            schemas.AddRange(ExtendedAttributeSchemas(
                attributes: ScimExtendedAttributeUtilities.UserAttributes(),
                tableName: ScimExtendedAttributeUtilities.UsersTableName,
                resourceName: "User"));
            schemas.AddRange(ExtendedAttributeSchemas(
                attributes: ScimExtendedAttributeUtilities.GroupAttributes(),
                tableName: ScimExtendedAttributeUtilities.GroupsTableName,
                resourceName: "Group"));
            return WithSysLog(() => ScimResponse.Content(ListResponse([.. schemas])));
        }

        [HttpGet("Schemas/{id}")]
        public ContentResult Schema(string id)
        {
            var schema = SchemaResource(id);
            return WithSysLog(() => schema == null
                ? ScimResponse.Error(404, "Schema not found")
                : ScimResponse.Content(schema));
        }

        private JObject ResourceTypeResource(string id)
        {
            if (string.Equals(id, "User", StringComparison.OrdinalIgnoreCase))
            {
                return UserResourceType();
            }
            if (string.Equals(id, "Group", StringComparison.OrdinalIgnoreCase))
            {
                return GroupResourceType();
            }
            return null;
        }

        private JObject UserResourceType()
        {
            var schemaExtensions = new JArray
            {
                new JObject
                {
                    ["schema"] = ScimSchemas.EnterpriseUser,
                    ["required"] = false
                }
            };
            AddSchemaExtensions(
                schemaExtensions: schemaExtensions,
                schemas: ScimExtendedAttributeUtilities.Schemas(
                    attributes: ScimExtendedAttributeUtilities.UserAttributes(),
                    tableName: ScimExtendedAttributeUtilities.UsersTableName));
            return new JObject
            {
                ["schemas"] = new JArray(ResourceTypeSchema),
                ["id"] = "User",
                ["name"] = "User",
                ["endpoint"] = "/Users",
                ["description"] = "User Account",
                ["schema"] = ScimSchemas.User,
                ["schemaExtensions"] = schemaExtensions,
                ["meta"] = Meta("ResourceType", "/ResourceTypes/User")
            };
        }

        private JObject GroupResourceType()
        {
            var resourceType = new JObject
            {
                ["schemas"] = new JArray(ResourceTypeSchema),
                ["id"] = "Group",
                ["name"] = "Group",
                ["endpoint"] = "/Groups",
                ["description"] = "Group",
                ["schema"] = ScimSchemas.Group,
                ["meta"] = Meta("ResourceType", "/ResourceTypes/Group")
            };
            var schemaExtensions = new JArray();
            AddSchemaExtensions(
                schemaExtensions: schemaExtensions,
                schemas: ScimExtendedAttributeUtilities.Schemas(
                    attributes: ScimExtendedAttributeUtilities.GroupAttributes(),
                    tableName: ScimExtendedAttributeUtilities.GroupsTableName));
            if (schemaExtensions.Count > 0)
            {
                resourceType["schemaExtensions"] = schemaExtensions;
            }
            return resourceType;
        }

        private JObject SchemaResource(string id)
        {
            if (IsSchemaId(id, ScimSchemas.User, "User"))
            {
                return UserSchema();
            }
            if (IsSchemaId(id, ScimSchemas.Group, "Group"))
            {
                return GroupSchema();
            }
            if (IsSchemaId(id, ScimSchemas.EnterpriseUser, "EnterpriseUser"))
            {
                return EnterpriseUserSchema();
            }
            return ExtendedAttributeSchemaResource(id);
        }

        private static void AddSchemaExtensions(JArray schemaExtensions, IEnumerable<string> schemas)
        {
            foreach (var schema in schemas)
            {
                schemaExtensions.Add(new JObject
                {
                    ["schema"] = schema,
                    ["required"] = false
                });
            }
        }

        private JObject ExtendedAttributeSchemaResource(string id)
        {
            return ExtendedAttributeSchemas(
                    attributes: ScimExtendedAttributeUtilities.UserAttributes(),
                    tableName: ScimExtendedAttributeUtilities.UsersTableName,
                    resourceName: "User")
                .Concat(ExtendedAttributeSchemas(
                    attributes: ScimExtendedAttributeUtilities.GroupAttributes(),
                    tableName: ScimExtendedAttributeUtilities.GroupsTableName,
                    resourceName: "Group"))
                .FirstOrDefault(schema => string.Equals(
                    schema["id"]?.ToString(),
                    id,
                    StringComparison.OrdinalIgnoreCase));
        }

        private IEnumerable<JObject> ExtendedAttributeSchemas(
            IEnumerable<Implem.ParameterAccessor.Parts.ScimExtendedAttribute> attributes,
            string tableName,
            string resourceName)
        {
            var list = attributes.ToList();
            return ScimExtendedAttributeUtilities.Schemas(list, tableName)
                .Select(schema => ExtendedAttributeSchema(
                    schema: schema,
                    attributes: list
                        .Where(attribute => string.Equals(
                            ScimExtendedAttributeUtilities.Schema(attribute, tableName),
                            schema,
                            StringComparison.OrdinalIgnoreCase)),
                    tableName: tableName,
                    resourceName: resourceName));
        }

        private JObject ExtendedAttributeSchema(
            string schema,
            IEnumerable<Implem.ParameterAccessor.Parts.ScimExtendedAttribute> attributes,
            string tableName,
            string resourceName)
        {
            return new JObject
            {
                ["schemas"] = new JArray(SchemaSchema),
                ["id"] = schema,
                ["name"] = $"{resourceName}ExtendedAttributes",
                ["description"] = $"Pleasanter {resourceName} extended attributes.",
                ["attributes"] = new JArray(attributes
                    .GroupBy(
                        attribute => ScimExtendedAttributeUtilities.Name(attribute),
                        StringComparer.OrdinalIgnoreCase)
                    .Select(group => group.First())
                    .Select(attribute => Attribute(
                        name: ScimExtendedAttributeUtilities.Name(attribute),
                        type: "string",
                        description:
                            $"Mapped to Pleasanter {tableName}.{ScimExtendedAttributeUtilities.ColumnName(attribute, tableName)}."))),
                ["meta"] = Meta("Schema", $"/Schemas/{schema}")
            };
        }

        private static bool IsSchemaId(string id, string schemaId, string shortName)
        {
            return string.Equals(id, schemaId, StringComparison.OrdinalIgnoreCase)
                || string.Equals(id, shortName, StringComparison.OrdinalIgnoreCase);
        }

        private JObject UserSchema()
        {
            return new JObject
            {
                ["schemas"] = new JArray(SchemaSchema),
                ["id"] = ScimSchemas.User,
                ["name"] = "User",
                ["description"] = "User Account",
                ["attributes"] = new JArray
                {
                    Attribute(
                        name: "userName",
                        type: "string",
                        description: "Unique identifier used to log in to Pleasanter.",
                        required: true,
                        uniqueness: "server"),
                    Attribute(
                        name: "name",
                        type: "complex",
                        description: "User name parts.",
                        subAttributes:
                        [
                            Attribute("familyName", "string"),
                            Attribute("givenName", "string")
                        ]),
                    Attribute("displayName", "string", "Display name."),
                    Attribute(
                        name: "active",
                        type: "boolean",
                        description: "false disables the Pleasanter user."),
                    Attribute(
                        name: "emails",
                        type: "complex",
                        description: "Mail addresses.",
                        multiValued: true,
                        subAttributes:
                        [
                            Attribute("value", "string"),
                            Attribute(
                                name: "type",
                                type: "string",
                                canonicalValues: new JArray("work")),
                            Attribute("primary", "boolean")
                        ]),
                    Attribute(
                        name: "externalId",
                        type: "string",
                        description: "Identifier assigned by the identity provider."),
                    Attribute(
                        name: "id",
                        type: "string",
                        description: "SCIM id assigned by Pleasanter.",
                        mutability: "readOnly",
                        returned: "always",
                        uniqueness: "server"),
                    Attribute(
                        name: "meta",
                        type: "complex",
                        description: "Resource metadata.",
                        mutability: "readOnly",
                        returned: "default",
                        subAttributes:
                        [
                            Attribute("resourceType", "string", mutability: "readOnly"),
                            Attribute("created", "dateTime", mutability: "readOnly"),
                            Attribute("lastModified", "dateTime", mutability: "readOnly"),
                            Attribute(
                                name: "location",
                                type: "reference",
                                mutability: "readOnly",
                                referenceTypes: new JArray("uri"))
                        ])
                },
                ["meta"] = Meta("Schema", $"/Schemas/{ScimSchemas.User}")
            };
        }

        private JObject GroupSchema()
        {
            return new JObject
            {
                ["schemas"] = new JArray(SchemaSchema),
                ["id"] = ScimSchemas.Group,
                ["name"] = "Group",
                ["description"] = "Group",
                ["attributes"] = new JArray
                {
                    Attribute(
                        name: "displayName",
                        type: "string",
                        description: "Group name.",
                        required: true),
                    Attribute(
                        name: "members",
                        type: "complex",
                        description: "User and nested Group members.",
                        multiValued: true,
                        subAttributes:
                        [
                            Attribute("value", "string"),
                            Attribute(
                                name: "$ref",
                                type: "reference",
                                referenceTypes: new JArray("User", "Group")),
                            Attribute("display", "string", mutability: "readOnly"),
                            Attribute(
                                name: "type",
                                type: "string",
                                canonicalValues: new JArray("User", "Group"))
                        ]),
                    Attribute(
                        name: "externalId",
                        type: "string",
                        description: "Identifier assigned by the identity provider."),
                    Attribute(
                        name: "id",
                        type: "string",
                        description: "SCIM id assigned by Pleasanter.",
                        mutability: "readOnly",
                        returned: "always",
                        uniqueness: "server"),
                    Attribute(
                        name: "meta",
                        type: "complex",
                        description: "Resource metadata.",
                        mutability: "readOnly",
                        returned: "default",
                        subAttributes:
                        [
                            Attribute("resourceType", "string", mutability: "readOnly"),
                            Attribute("created", "dateTime", mutability: "readOnly"),
                            Attribute("lastModified", "dateTime", mutability: "readOnly"),
                            Attribute(
                                name: "location",
                                type: "reference",
                                mutability: "readOnly",
                                referenceTypes: new JArray("uri"))
                        ])
                },
                ["meta"] = Meta("Schema", $"/Schemas/{ScimSchemas.Group}")
            };
        }

        private JObject EnterpriseUserSchema()
        {
            return new JObject
            {
                ["schemas"] = new JArray(SchemaSchema),
                ["id"] = ScimSchemas.EnterpriseUser,
                ["name"] = "EnterpriseUser",
                ["description"] = "Enterprise User",
                ["attributes"] = new JArray
                {
                    Attribute(
                        name: "department",
                        type: "string",
                        description: "Mapped to Pleasanter DeptCode and DeptName."),
                    Attribute(
                        name: "employeeNumber",
                        type: "string",
                        description: "Mapped to Pleasanter Users.UserCode.")
                },
                ["meta"] = Meta("Schema", $"/Schemas/{ScimSchemas.EnterpriseUser}")
            };
        }

        private static JObject ListResponse(params JObject[] resources)
        {
            return new JObject
            {
                ["schemas"] = new JArray(ListResponseSchema),
                ["totalResults"] = resources.Length,
                ["startIndex"] = 1,
                ["itemsPerPage"] = resources.Length,
                ["Resources"] = new JArray(resources)
            };
        }

        private static JObject Attribute(
            string name,
            string type,
            string description = null,
            bool required = false,
            bool multiValued = false,
            string mutability = "readWrite",
            string returned = "default",
            string uniqueness = "none",
            JArray subAttributes = null,
            JArray canonicalValues = null,
            JArray referenceTypes = null)
        {
            var attribute = new JObject
            {
                ["name"] = name,
                ["type"] = type,
                ["multiValued"] = multiValued,
                ["description"] = description ?? name,
                ["required"] = required,
                ["caseExact"] = false,
                ["mutability"] = mutability,
                ["returned"] = returned,
                ["uniqueness"] = uniqueness
            };
            if (subAttributes != null)
            {
                attribute["subAttributes"] = subAttributes;
            }
            if (canonicalValues != null)
            {
                attribute["canonicalValues"] = canonicalValues;
            }
            if (referenceTypes != null)
            {
                attribute["referenceTypes"] = referenceTypes;
            }
            return attribute;
        }

        private JObject Meta(string resourceType, string location)
        {
            var pathBase = Request.PathBase.Value?.TrimEnd('/');
            return new JObject
            {
                ["resourceType"] = resourceType,
                ["location"] = $"{pathBase}/scim/v2{location}"
            };
        }
    }
}
