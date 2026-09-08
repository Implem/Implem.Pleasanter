using Newtonsoft.Json.Linq;

namespace Implem.Pleasanter.Libraries.Scim
{
    public static class ScimOpenApiDocument
    {
        public static JObject Create(string serverUrl = null)
        {
            return new JObject
            {
                ["openapi"] = "3.0.1",
                ["info"] = new JObject
                {
                    ["title"] = "Pleasanter SCIM API",
                    ["version"] = "v1",
                    ["description"] = "SCIM 2.0 endpoints for User and Group provisioning."
                },
                ["servers"] = new JArray(
                    new JObject
                    {
                        ["url"] = string.IsNullOrWhiteSpace(serverUrl)
                            ? "/"
                            : serverUrl.TrimEnd('/')
                    }),
                ["security"] = new JArray(SecurityRequirement()),
                ["paths"] = Paths(),
                ["components"] = Components(),
                ["tags"] = new JArray(
                    new JObject { ["name"] = "Users" },
                    new JObject { ["name"] = "Groups" },
                    new JObject { ["name"] = "Metadata" })
            };
        }

        private static JObject Paths()
        {
            return new JObject
            {
                ["/scim/v2/ServiceProviderConfig"] = new JObject
                {
                    ["get"] = Operation(
                        tag: "Metadata",
                        summary: "Get ServiceProviderConfig",
                        description: "Returns the SCIM features supported by Pleasanter.",
                        responses: Responses(
                            ("200", "OK", "ScimServiceProviderConfig"),
                            ("401", "Unauthorized", "ScimError")))
                },
                ["/scim/v2/ResourceTypes"] = new JObject
                {
                    ["get"] = Operation(
                        tag: "Metadata",
                        summary: "List ResourceTypes",
                        description: "Returns available SCIM resource types.",
                        responses: ListResponses())
                },
                ["/scim/v2/ResourceTypes/{id}"] = new JObject
                {
                    ["get"] = Operation(
                        tag: "Metadata",
                        summary: "Get ResourceType",
                        description: "Returns User or Group resource type metadata.",
                        parameters: new JArray(IdParameter("ResourceType id. Example: User or Group.")),
                        responses: Responses(
                            ("200", "OK", "ScimResourceType"),
                            ("401", "Unauthorized", "ScimError"),
                            ("404", "Not Found", "ScimError")))
                },
                ["/scim/v2/Schemas"] = new JObject
                {
                    ["get"] = Operation(
                        tag: "Metadata",
                        summary: "List Schemas",
                        description: "Returns SCIM schema definitions advertised by Pleasanter.",
                        responses: ListResponses())
                },
                ["/scim/v2/Schemas/{id}"] = new JObject
                {
                    ["get"] = Operation(
                        tag: "Metadata",
                        summary: "Get Schema",
                        description: "Returns a SCIM schema definition.",
                        parameters: new JArray(IdParameter("Schema id.")),
                        responses: Responses(
                            ("200", "OK", "ScimSchema"),
                            ("401", "Unauthorized", "ScimError"),
                            ("404", "Not Found", "ScimError")))
                },
                ["/scim/v2/Users"] = new JObject
                {
                    ["get"] = Operation(
                        tag: "Users",
                        summary: "List Users",
                        description: "Searches SCIM Users. Filters for id, externalId and userName are supported.",
                        parameters: PagingParameters(includeExcludedAttributes: false),
                        responses: ListResponses()),
                    ["post"] = Operation(
                        tag: "Users",
                        summary: "Create User",
                        description: "Creates a Pleasanter user or links an existing user to SCIM.",
                        requestBody: RequestBody("ScimUser"),
                        responses: Responses(
                            ("201", "Created", "ScimUser"),
                            ("400", "Bad Request", "ScimError"),
                            ("401", "Unauthorized", "ScimError"),
                            ("409", "Conflict", "ScimError")))
                },
                ["/scim/v2/Users/{id}"] = new JObject
                {
                    ["get"] = Operation(
                        tag: "Users",
                        summary: "Get User",
                        description: "Returns a SCIM User by Pleasanter SCIM id.",
                        parameters: new JArray(IdParameter("Pleasanter SCIM user id.")),
                        responses: Responses(
                            ("200", "OK", "ScimUser"),
                            ("401", "Unauthorized", "ScimError"),
                            ("404", "Not Found", "ScimError"))),
                    ["put"] = Operation(
                        tag: "Users",
                        summary: "Replace User",
                        description: "Replaces a SCIM User.",
                        parameters: new JArray(IdParameter("Pleasanter SCIM user id.")),
                        requestBody: RequestBody("ScimUser"),
                        responses: Responses(
                            ("200", "OK", "ScimUser"),
                            ("400", "Bad Request", "ScimError"),
                            ("401", "Unauthorized", "ScimError"),
                            ("404", "Not Found", "ScimError"))),
                    ["patch"] = Operation(
                        tag: "Users",
                        summary: "Patch User",
                        description: "Updates selected User attributes. PATCH op values are compared case-insensitively.",
                        parameters: new JArray(IdParameter("Pleasanter SCIM user id.")),
                        requestBody: RequestBody("ScimPatchRequest"),
                        responses: Responses(
                            ("200", "OK", "ScimUser"),
                            ("400", "Bad Request", "ScimError"),
                            ("401", "Unauthorized", "ScimError"),
                            ("404", "Not Found", "ScimError"))),
                    ["delete"] = Operation(
                        tag: "Users",
                        summary: "Disable User",
                        description: "Disables the Pleasanter user. The row is not physically deleted.",
                        parameters: new JArray(IdParameter("Pleasanter SCIM user id.")),
                        responses: Responses(
                            ("204", "No Content", null),
                            ("401", "Unauthorized", "ScimError"),
                            ("404", "Not Found", "ScimError")))
                },
                ["/scim/v2/Groups"] = new JObject
                {
                    ["get"] = Operation(
                        tag: "Groups",
                        summary: "List Groups",
                        description: "Searches SCIM Groups. Filters for id, externalId and displayName are supported.",
                        parameters: PagingParameters(includeExcludedAttributes: true),
                        responses: ListResponses()),
                    ["post"] = Operation(
                        tag: "Groups",
                        summary: "Create Group",
                        description: "Creates a Pleasanter group or restores a stopped SCIM group.",
                        requestBody: RequestBody("ScimGroup"),
                        responses: Responses(
                            ("201", "Created", "ScimGroup"),
                            ("400", "Bad Request", "ScimError"),
                            ("401", "Unauthorized", "ScimError"),
                            ("409", "Conflict", "ScimError")))
                },
                ["/scim/v2/Groups/{id}"] = new JObject
                {
                    ["get"] = Operation(
                        tag: "Groups",
                        summary: "Get Group",
                        description: "Returns a SCIM Group by Pleasanter SCIM id.",
                        parameters: new JArray(
                            IdParameter("Pleasanter SCIM group id."),
                            ExcludedAttributesParameter()),
                        responses: Responses(
                            ("200", "OK", "ScimGroup"),
                            ("401", "Unauthorized", "ScimError"),
                            ("404", "Not Found", "ScimError"))),
                    ["put"] = Operation(
                        tag: "Groups",
                        summary: "Replace Group",
                        description: "Replaces a SCIM Group and its members.",
                        parameters: new JArray(IdParameter("Pleasanter SCIM group id.")),
                        requestBody: RequestBody("ScimGroup"),
                        responses: Responses(
                            ("200", "OK", "ScimGroup"),
                            ("400", "Bad Request", "ScimError"),
                            ("401", "Unauthorized", "ScimError"),
                            ("404", "Not Found", "ScimError"))),
                    ["patch"] = Operation(
                        tag: "Groups",
                        summary: "Patch Group",
                        description: "Updates Group attributes and members. User and nested Group members are supported.",
                        parameters: new JArray(IdParameter("Pleasanter SCIM group id.")),
                        requestBody: RequestBody("ScimPatchRequest"),
                        responses: Responses(
                            ("204", "No Content", null),
                            ("400", "Bad Request", "ScimError"),
                            ("401", "Unauthorized", "ScimError"),
                            ("404", "Not Found", "ScimError"))),
                    ["delete"] = Operation(
                        tag: "Groups",
                        summary: "Disable Group",
                        description: "Disables the Pleasanter group. The row is not physically deleted.",
                        parameters: new JArray(IdParameter("Pleasanter SCIM group id.")),
                        responses: Responses(
                            ("204", "No Content", null),
                            ("401", "Unauthorized", "ScimError"),
                            ("404", "Not Found", "ScimError")))
                }
            };
        }

        private static JObject Components()
        {
            return new JObject
            {
                ["securitySchemes"] = new JObject
                {
                    ["ScimBearer"] = new JObject
                    {
                        ["type"] = "http",
                        ["scheme"] = "bearer",
                        ["description"] = "SCIM Bearer Token. Set the Authorization header as: Bearer {token}"
                    }
                },
                ["schemas"] = new JObject
                {
                    ["ScimUser"] = ObjectSchema(
                        required: new JArray("schemas", "externalId", "userName"),
                        properties: new JObject
                        {
                            ["schemas"] = StringArray("SCIM schema URNs."),
                            ["id"] = StringProperty("Pleasanter SCIM id.", readOnly: true),
                            ["externalId"] = StringProperty("Identity provider object id."),
                            ["userName"] = StringProperty("Login id."),
                            ["name"] = Ref("ScimName"),
                            ["displayName"] = StringProperty("Display name."),
                            ["active"] = new JObject { ["type"] = "boolean" },
                            ["emails"] = ArrayOf(Ref("ScimEmail")),
                            [ScimSchemas.EnterpriseUser] = Ref("ScimEnterpriseUser"),
                            ["meta"] = Ref("ScimMeta")
                        },
                        additionalProperties: true),
                    ["ScimName"] = ObjectSchema(
                        properties: new JObject
                        {
                            ["familyName"] = StringProperty("Family name."),
                            ["givenName"] = StringProperty("Given name.")
                        }),
                    ["ScimEmail"] = ObjectSchema(
                        properties: new JObject
                        {
                            ["value"] = StringProperty("Mail address."),
                            ["type"] = StringProperty("Mail type. Pleasanter uses work."),
                            ["primary"] = new JObject { ["type"] = "boolean" }
                        }),
                    ["ScimEnterpriseUser"] = ObjectSchema(
                        properties: new JObject
                        {
                            ["department"] = StringProperty("Mapped to Pleasanter Dept."),
                            ["employeeNumber"] = StringProperty("Mapped to Pleasanter Users.UserCode.")
                        }),
                    ["ScimGroup"] = ObjectSchema(
                        required: new JArray("schemas", "externalId", "displayName"),
                        properties: new JObject
                        {
                            ["schemas"] = StringArray("SCIM schema URNs."),
                            ["id"] = StringProperty("Pleasanter SCIM id.", readOnly: true),
                            ["externalId"] = StringProperty("Identity provider object id."),
                            ["displayName"] = StringProperty("Group name."),
                            ["members"] = ArrayOf(Ref("ScimMember")),
                            ["meta"] = Ref("ScimMeta")
                        },
                        additionalProperties: true),
                    ["ScimMember"] = ObjectSchema(
                        properties: new JObject
                        {
                            ["value"] = StringProperty("SCIM id of a User or Group."),
                            ["display"] = StringProperty("Display value.", readOnly: true),
                            ["type"] = StringProperty("User or Group."),
                            ["$ref"] = StringProperty("Resource URL.", readOnly: true)
                        }),
                    ["ScimPatchRequest"] = ObjectSchema(
                        required: new JArray("schemas", "Operations"),
                        properties: new JObject
                        {
                            ["schemas"] = StringArray("SCIM schema URNs."),
                            ["Operations"] = ArrayOf(Ref("ScimPatchOperation"))
                        }),
                    ["ScimPatchOperation"] = ObjectSchema(
                        required: new JArray("op"),
                        properties: new JObject
                        {
                            ["op"] = StringProperty("Add, Replace or Remove."),
                            ["path"] = StringProperty("Target attribute path."),
                            ["value"] = new JObject
                            {
                                ["description"] = "PATCH value. The shape depends on the target path."
                            }
                        }),
                    ["ScimListResponse"] = ObjectSchema(
                        properties: new JObject
                        {
                            ["schemas"] = StringArray("SCIM schema URNs."),
                            ["totalResults"] = IntegerProperty("Total result count."),
                            ["Resources"] = ArrayOf(new JObject
                            {
                                ["oneOf"] = new JArray(
                                    Ref("ScimUser"),
                                    Ref("ScimGroup"),
                                    Ref("ScimResourceType"),
                                    Ref("ScimSchema"))
                            }),
                            ["startIndex"] = IntegerProperty("Start index."),
                            ["itemsPerPage"] = IntegerProperty("Returned item count.")
                        }),
                    ["ScimError"] = ObjectSchema(
                        properties: new JObject
                        {
                            ["schemas"] = StringArray("SCIM schema URNs."),
                            ["status"] = StringProperty("HTTP status code."),
                            ["scimType"] = StringProperty("SCIM error type."),
                            ["detail"] = StringProperty("Error detail.")
                        }),
                    ["ScimMeta"] = ObjectSchema(
                        properties: new JObject
                        {
                            ["resourceType"] = StringProperty("Resource type.", readOnly: true),
                            ["created"] = StringProperty("Created time.", format: "date-time", readOnly: true),
                            ["lastModified"] = StringProperty("Last modified time.", format: "date-time", readOnly: true),
                            ["location"] = StringProperty("Resource URL.", readOnly: true)
                        }),
                    ["ScimServiceProviderConfig"] = ObjectSchema(),
                    ["ScimResourceType"] = ObjectSchema(),
                    ["ScimSchema"] = ObjectSchema()
                }
            };
        }

        private static JObject Operation(
            string tag,
            string summary,
            string description,
            JObject responses,
            JArray parameters = null,
            JObject requestBody = null)
        {
            var operation = new JObject
            {
                ["tags"] = new JArray(tag),
                ["summary"] = summary,
                ["description"] = description,
                ["responses"] = responses
            };
            if (parameters != null)
            {
                operation["parameters"] = parameters;
            }
            if (requestBody != null)
            {
                operation["requestBody"] = requestBody;
            }
            return operation;
        }

        private static JArray PagingParameters(bool includeExcludedAttributes)
        {
            var parameters = new JArray
            {
                QueryParameter("startIndex", "integer", "1-based start index.", "1"),
                QueryParameter("count", "integer", "Maximum number of resources to return.", "100"),
                QueryParameter("filter", "string", "SCIM eq filter.")
            };
            if (includeExcludedAttributes)
            {
                parameters.Add(ExcludedAttributesParameter());
            }
            return parameters;
        }

        private static JObject IdParameter(string description)
        {
            return new JObject
            {
                ["name"] = "id",
                ["in"] = "path",
                ["required"] = true,
                ["description"] = description,
                ["schema"] = new JObject { ["type"] = "string" }
            };
        }

        private static JObject QueryParameter(
            string name,
            string type,
            string description,
            string defaultValue = null)
        {
            var schema = new JObject { ["type"] = type };
            if (defaultValue != null)
            {
                schema["default"] = type == "integer"
                    ? int.Parse(defaultValue)
                    : defaultValue;
            }
            return new JObject
            {
                ["name"] = name,
                ["in"] = "query",
                ["required"] = false,
                ["description"] = description,
                ["schema"] = schema
            };
        }

        private static JObject ExcludedAttributesParameter()
        {
            return QueryParameter(
                name: "excludedAttributes",
                type: "string",
                description: "Use members to omit Group members from the response.");
        }

        private static JObject RequestBody(string schemaName)
        {
            return new JObject
            {
                ["required"] = true,
                ["content"] = Content(schemaName)
            };
        }

        private static JObject ListResponses()
        {
            return Responses(
                ("200", "OK", "ScimListResponse"),
                ("401", "Unauthorized", "ScimError"));
        }

        private static JObject Responses(params (string status, string description, string schemaName)[] values)
        {
            var responses = new JObject();
            foreach (var (status, description, schemaName) in values)
            {
                var response = new JObject
                {
                    ["description"] = description
                };
                if (schemaName != null)
                {
                    response["content"] = Content(schemaName);
                }
                responses[status] = response;
            }
            return responses;
        }

        private static JObject Content(string schemaName)
        {
            return new JObject
            {
                ["application/scim+json"] = new JObject
                {
                    ["schema"] = Ref(schemaName)
                }
            };
        }

        private static JObject Ref(string schemaName)
        {
            return new JObject
            {
                ["$ref"] = $"#/components/schemas/{schemaName}"
            };
        }

        private static JObject ObjectSchema(
            JArray required = null,
            JObject properties = null,
            bool? additionalProperties = null)
        {
            var schema = new JObject
            {
                ["type"] = "object"
            };
            if (required != null)
            {
                schema["required"] = required;
            }
            if (properties != null)
            {
                schema["properties"] = properties;
            }
            if (additionalProperties.HasValue)
            {
                schema["additionalProperties"] = additionalProperties.Value;
            }
            return schema;
        }

        private static JObject ArrayOf(JObject items)
        {
            return new JObject
            {
                ["type"] = "array",
                ["items"] = items
            };
        }

        private static JObject StringArray(string description)
        {
            return new JObject
            {
                ["type"] = "array",
                ["description"] = description,
                ["items"] = new JObject { ["type"] = "string" }
            };
        }

        private static JObject StringProperty(
            string description,
            string format = null,
            bool readOnly = false)
        {
            var schema = new JObject
            {
                ["type"] = "string",
                ["description"] = description
            };
            if (format != null)
            {
                schema["format"] = format;
            }
            if (readOnly)
            {
                schema["readOnly"] = true;
            }
            return schema;
        }

        private static JObject IntegerProperty(string description)
        {
            return new JObject
            {
                ["type"] = "integer",
                ["format"] = "int32",
                ["description"] = description
            };
        }

        private static JObject SecurityRequirement()
        {
            return new JObject
            {
                ["ScimBearer"] = new JArray()
            };
        }
    }
}
