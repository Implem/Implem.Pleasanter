using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Implem.Pleasanter.MCP.Utilities
{
    public class ColumnFilterHashJsonConverter :
        JsonConverter<Dictionary<string, List<string>>>
    {
        public override bool HandleNull => true;

        public override Dictionary<string, List<string>> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("columnFilterHash は JSON オブジェクトで指定してください。");
            }

            var result = new Dictionary<string, List<string>>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return result;
                }
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("columnFilterHash のキーが不正です。");
                }

                var key = reader.GetString();
                if (reader.Read() == false)
                {
                    throw new JsonException("columnFilterHash の値がありません。");
                }
                var value = ReadValue(ref reader);
                if (result.TryAdd(
                    key,
                    value) == false)
                {
                    throw new JsonException("columnFilterHash に重複したキーがあります。");
                }
            }

            throw new JsonException("columnFilterHash の JSON が完了していません。");
        }

        private static List<string> ReadValue(ref Utf8JsonReader reader)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return new List<string> { reader.GetString() };
            }
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                return ReadArray(ref reader);
            }
            throw new JsonException("columnFilterHash の値は文字列または文字列配列で指定してください。");
        }

        private static List<string> ReadArray(ref Utf8JsonReader reader)
        {
            var result = new List<string>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return result;
                }
                if (reader.TokenType != JsonTokenType.String)
                {
                    throw new JsonException("columnFilterHash の配列要素は文字列で指定してください。");
                }
                result.Add(reader.GetString());
            }

            throw new JsonException("columnFilterHash の配列が完了していません。");
        }

        public override void Write(
            Utf8JsonWriter writer,
            Dictionary<string, List<string>> value,
            JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();
            foreach (var pair in value)
            {
                writer.WritePropertyName(pair.Key);
                JsonSerializer.Serialize(
                    writer,
                    pair.Value,
                    options);
            }
            writer.WriteEndObject();
        }
    }
}
