using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Implem.ParameterAccessor.Parts
{
    public class ContentSecurityPolicy
    {
        public bool Enabled { get; set; } = false;
        public bool ReportOnlyEnabled { get; set; } = true;
        public List<ContentSecurityPolicyValues> Values { get; set; }

        public bool IsSettings() => Values?.Count > 0;

        public string GetHeaderValues(
            string nonce,
            bool isDevelopment)
        {
            if (!IsSettings())
            {
                return string.Empty;
            }
            var policy = Values.First();
            return string.Join(
                separator: " ",
                values: typeof(ContentSecurityPolicyValues)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(prop =>
                    {
                        var attr = prop.GetCustomAttribute<JsonPropertyAttribute>();
                        var key = attr?.PropertyName ?? prop.Name;
                        var value = prop.GetValue(policy)?.ToString();
                        if (string.IsNullOrEmpty(value))
                        {
                            return null;
                        }
                        string resultValue = key switch
                        {
                            "script-src" or "style-src" => $"{value} 'nonce-{nonce}'",
                            "connect-src" when isDevelopment => $"{value} http://localhost:* ws://localhost:* wss://localhost:*",
                            _ => value
                        };
                        return $"{key} {resultValue};";
                    })
                    .Where(s => s != null)
            );
        }
    }
}
