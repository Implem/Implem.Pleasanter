using System;
using Newtonsoft.Json;

namespace Implem.ParameterAccessor.Parts
{
    public class CaptchaConfig
    {
        public enum Types
        {
            None,
            ReCaptchaV2,
            RecaptchaV3,
            Turnstile
        }

        // 不正値は None にフォールバック（Newtownsoft 用）
        [JsonConverter(typeof(CaptchaTypesJsonConverter))]
        public Types Type = Types.None;
        public string SiteKey;
        public string SecretKey;
        public RecaptchaV3 RecaptchaV3;
    }

    public class RecaptchaV3
    {
        public double DefaultScoreThreshold = 0.5;
    }

    /// <summary>
    /// 未知/不正な列挙値を例外にせず None にフォールバックする Newtonsoft.Json 用コンバータ
    /// </summary>
    internal sealed class CaptchaTypesJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(CaptchaConfig.Types) || objectType == typeof(CaptchaConfig.Types?);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader.TokenType == JsonToken.String)
                {
                    var str = (reader.Value as string)?.Trim();
                    if (!string.IsNullOrEmpty(str)
                        && Enum.TryParse<CaptchaConfig.Types>(str, true, out var parsed))
                    {
                        return parsed;
                    }
                    return CaptchaConfig.Types.None;
                }
                if (reader.TokenType == JsonToken.Integer)
                {
                    var intVal = Convert.ToInt32(reader.Value);
                    if (Enum.IsDefined(typeof(CaptchaConfig.Types), intVal))
                    {
                        return (CaptchaConfig.Types)intVal;
                    }
                    return CaptchaConfig.Types.None;
                }
            }
            catch
            {
                // 例外時もフォールバック
            }
            return CaptchaConfig.Types.None;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // 可読性維持のため文字列で出力
            writer.WriteValue(value?.ToString());
        }
    }
}