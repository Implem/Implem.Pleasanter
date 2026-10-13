using System.Collections.Generic;
using Implem.Pleasanter.MCP.Tools;
using ModelContextProtocol.Protocol;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Implem.Pleasanter.MCP.Utilities
{
    public static class ColumnFilterHashInputSchemaUtilities
    {
        private const string ColumnFilterHashPropertyName = "columnFilterHash";

        public static void Correct(IList<Tool> tools)
        {
            if (tools == null)
            {
                return;
            }

            foreach (var tool in tools)
            {
                if (tool?.Name != nameof(ViewsTool.CreateViewJson)
                    || tool.InputSchema.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                if (IsCorrected(inputSchema: tool.InputSchema))
                {
                    continue;
                }

                var schema = JsonNode.Parse(tool.InputSchema.GetRawText()) as JsonObject;
                var properties = schema?["properties"] as JsonObject;
                var columnFilterHash = properties?[ColumnFilterHashPropertyName] as JsonObject;
                if (columnFilterHash == null)
                {
                    continue;
                }

                columnFilterHash["type"] = new JsonArray { "object", "null" };
                columnFilterHash["additionalProperties"] = new JsonObject
                {
                    ["oneOf"] = new JsonArray
                    {
                        new JsonObject
                        {
                            ["type"] = "string"
                        },
                        new JsonObject
                        {
                            ["type"] = "array",
                            ["items"] = new JsonObject
                            {
                                ["type"] = "string"
                            }
                        }
                    }
                };
                tool.InputSchema = JsonSerializer.SerializeToElement(schema);
            }
        }

        private static bool IsCorrected(JsonElement inputSchema)
        {
            return inputSchema.TryGetProperty(
                    propertyName: "properties",
                    value: out var properties)
                && properties.TryGetProperty(
                    propertyName: ColumnFilterHashPropertyName,
                    value: out var columnFilterHash)
                && columnFilterHash.TryGetProperty(
                    propertyName: "additionalProperties",
                    value: out var additionalProperties)
                && additionalProperties.TryGetProperty(
                    propertyName: "oneOf",
                    value: out _);
        }
    }
}
