using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Implem.Libraries.Utilities
{
    public static class Jsons
    {
        public static string ToJson(
            this object obj,
            DefaultValueHandling defaultValueHandling = DefaultValueHandling.Include,
            NullValueHandling nullValueHandling = NullValueHandling.Ignore,
            Formatting formatting = Formatting.None)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = nullValueHandling;
            settings.DefaultValueHandling = defaultValueHandling;
            settings.Formatting = formatting;
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static T Deserialize<T>(this string str)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(str);
            }
            catch
            {
                return default(T);
            }
        }

        public static T Copy<T>(this T self)
        {
            return self != null
                ? self.ToJson().Deserialize<T>()
                : default(T);
        }

        public static string BuildPatch(string a, string b, string typeName)
        {
            var type = Type.GetType($"Implem.ParameterAccessor.Parts.{typeName}, Implem.ParameterAccessor");
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error,
            };
            var tokenA = JToken.FromObject(JsonConvert.DeserializeObject(a, type, settings));
            var tokenB = JToken.FromObject(JsonConvert.DeserializeObject(b, type, settings));
            var ops = new JArray();
            if (tokenA is JObject objA && tokenB is JObject objB)
            {
                CompareObjects(objA, objB, "", ops);
            }
            else if (tokenA is JArray arrA && tokenB is JArray arrB)
            {
                CompareArrays(arrA, arrB, "", ops);
            }
            return ops.ToString(Formatting.Indented);
        }
        
        public static string ApplyPatch(string original, string patch, string typeName)
        {
            var type = Type.GetType($"Implem.ParameterAccessor.Parts.{typeName}, Implem.ParameterAccessor");
            var token = JToken.FromObject(JsonConvert.DeserializeObject(original, type));
            var ops = JArray.Parse(patch);
            foreach (var op in ops)
            {
                var opType = op["op"].ToString();
                var segments = op["path"].ToString().TrimStart('/').Split('/');
                for (var i = 0; i < segments.Length; i++)
                {
                    segments[i] = UnescapeJsonPointer(segment: segments[i]);
                }
                var value = op["value"];
                ApplyOperation(token, opType, segments, 0, value);
            }
            return token.ToString(Formatting.Indented);
        }

        public static string MergeArrayUnion(string original, string target, string propertyName)
        {
            var originalObject = JObject.Parse(original);
            var originalArray = originalObject[propertyName] as JArray;
            if (originalArray == null
                || originalArray.Count == 0)
            {
                return target;
            }
            var targetObject = JObject.Parse(target);
            var targetToken = targetObject[propertyName];
            JArray targetArray;
            if (targetToken == null
                || targetToken.Type == JTokenType.Null)
            {
                targetArray = new JArray();
                targetObject[propertyName] = targetArray;
            }
            else if (targetToken is JArray array)
            {
                targetArray = array;
            }
            else
            {
                return target;
            }
            var mergedArray = new JArray();
            foreach (var token in originalArray)
            {
                mergedArray.Add(token.DeepClone());
            }
            foreach (var token in targetArray)
            {
                if (originalArray.Any(o => JToken.DeepEquals(o, token)))
                {
                    continue;
                }
                if (mergedArray.Any(o => JToken.DeepEquals(o, token)))
                {
                    continue;
                }
                mergedArray.Add(token.DeepClone());
            }
            targetObject[propertyName] = mergedArray;
            return targetObject.ToString(Formatting.Indented);
        }

        private static void ApplyOperation(JToken current, string op, string[] segments, int depth, JToken value)
        {
            var key = segments[depth];
            var isLast = depth == segments.Length - 1;
            if (current is JObject obj)
            {
                if (isLast)
                {
                    switch (op)
                    {
                        case "add":
                        case "replace":
                            obj[key] = value.DeepClone();
                            break;
                        case "remove":
                            obj.Remove(key);
                            break;
                    }
                }
                else
                {
                    ApplyOperation(obj[key], op, segments, depth + 1, value);
                }
            }
            else if (current is JArray arr)
            {
                if (key == "-")
                {
                    arr.Add(value.DeepClone());
                }
                else
                {
                    var index = int.Parse(key);
                    if (isLast)
                    {
                        switch (op)
                        {
                            case "add":
                                arr.Insert(index, value.DeepClone());
                                break;
                            case "replace":
                                arr[index] = value.DeepClone();
                                break;
                            case "remove":
                                arr.RemoveAt(index);
                                break;
                        }
                    }
                    else
                    {
                        ApplyOperation(arr[index], op, segments, depth + 1, value);
                    }
                }
            }
        }

        private static string EscapeJsonPointer(string segment)
        {
            return segment
                .Replace("~", "~0")
                .Replace("/", "~1");
        }

        private static string UnescapeJsonPointer(string segment)
        {
            return segment
                .Replace("~1", "/")
                .Replace("~0", "~");
        }

        private static void CompareObjects(JObject a, JObject b, string path, JArray ops)
        {
            foreach (var prop in b.Properties())
            {
                var childPath = $"{path}/{EscapeJsonPointer(prop.Name)}";
                var aVal = a[prop.Name];
                if (aVal == null)
                    ops.Add(Op("add", childPath, prop.Value));
                else if (aVal.Type == JTokenType.Object && prop.Value.Type == JTokenType.Object)
                    CompareObjects((JObject)aVal, (JObject)prop.Value, childPath, ops);
                else if (aVal.Type == JTokenType.Array && prop.Value.Type == JTokenType.Array)
                    CompareArrays((JArray)aVal, (JArray)prop.Value, childPath, ops);
                else if (!JToken.DeepEquals(aVal, prop.Value))
                    ops.Add(Op("replace", childPath, prop.Value));
            }
            foreach (var prop in a.Properties())
                if (b[prop.Name] == null)
                    ops.Add(Op("remove", $"{path}/{EscapeJsonPointer(prop.Name)}", null));
        }

        private static void CompareArrays(JArray a, JArray b, string path, JArray ops)
        {
            var min = Math.Min(a.Count, b.Count);
            for (int i = 0; i < min; i++)
            {
                var childPath = $"{path}/{i}";
                if (a[i].Type == JTokenType.Object && b[i].Type == JTokenType.Object)
                    CompareObjects((JObject)a[i], (JObject)b[i], childPath, ops);
                else if (!JToken.DeepEquals(a[i], b[i]))
                    ops.Add(Op("replace", childPath, b[i]));
            }
            for (int i = min; i < b.Count; i++)
                ops.Add(Op("add", $"{path}/-", b[i]));
            for (int i = a.Count - 1; i >= min; i--)
                ops.Add(Op("remove", $"{path}/{i}", null));
        }

        private static JObject Op(string op, string path, JToken value)
        {
            var o = new JObject { ["op"] = op, ["path"] = path };
            if (value != null) o["value"] = value.DeepClone();
            return o;
        }
    }
}