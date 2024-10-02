using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Implem.PleasanterSetup
{
    public static class Jsons
    {
        public static string ToJson(
            this object obj,
            DefaultValueHandling defaultValueHandling = DefaultValueHandling.Include,
            Formatting formatting = Formatting.None)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
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

        public static string Merge(string source, string dest)
        {
            var destToken = JToken.Parse(dest);
            var sourceToken = JToken.Parse(source);
            MergeToken(sourceToken, destToken);
            return SerializeWithIndent(destToken);
        }

        public static string SerializeWithIndent(object obj, int indent = 4)
        {
            var sb = new StringBuilder();
            using (var writer = new System.IO.StringWriter(sb))
            using (var jsonWriter = new JsonTextWriter(writer)
            {
                Formatting = Formatting.Indented,
                Indentation = indent,
            })
            {
                new JsonSerializer().Serialize(jsonWriter, obj);
            }
            return sb.ToString();
        }

        private static void MergeToken(JToken sourceToken, JToken destToken)
        {
            switch (destToken.Type)
            {
                case JTokenType.Property:
                    var destProp = (JProperty)destToken;
                    var sourceProp = (JProperty)sourceToken;
                    if (destProp.Value.Type == JTokenType.Object
                        || destProp.Value.Type == JTokenType.Array)
                    {
                        if (sourceProp.Value.Type == JTokenType.Null)
                        {
                            destProp.Value = sourceProp.Value;
                        }
                        else
                        {
                            MergeToken(sourceProp.Value, destProp.Value);
                        }
                    }
                    else
                    {
                        destProp.Value = sourceProp.Value;
                    }
                    break;

                case JTokenType.Array:
                    var destArray = (JArray)destToken;
                    var sourceArray = (JArray)sourceToken;

                    var template = destArray.FirstOrDefault();
                    if (template?.Type == JTokenType.Object)
                    {
                        //配列の中身がオブジェクトの場合
                        //マージ先(dest)側に要素が複数ある場合は、マージ元(source)側と同じインデックスのオブジェクトをテンプレートにする
                        //マージ元(source)側の方が要素数が多い場合は最後の要素がテンプレートとなる
                        var copyArray = destArray.ToList();
                        destArray.Clear();
                        for (int i = 0; i < sourceArray.Count; i++)
                        {
                            template = i < copyArray.Count ? copyArray[i] : template;
                            var dest = template.DeepClone();
                            MergeToken(sourceArray[i], dest);
                            destArray.Add(dest);
                        }
                    }
                    else if (template?.Type == JTokenType.Array)
                    {
                        //配列の中身が配列の場合
                        destArray.Clear();
                        foreach (var source in sourceArray)
                        {
                            var dest = template.DeepClone();
                            MergeToken(source, dest);
                            destArray.Add(dest);
                        }
                    }
                    else
                    {
                        //配列の中身が値の場合
                        destArray.Clear();
                        foreach (var item in sourceArray)
                        {
                            destArray.Add(item);
                        }
                    }
                    break;
                case JTokenType.Object:
                    var destObj = (JObject)destToken;
                    var sourceObj = (JObject)sourceToken;
                    //マージ元(source)側にしかないプロパティをマージ先(dest)側にコピー
                    foreach (var s in sourceObj.Properties())
                    {
                        if (!destObj.ContainsKey(s.Name))
                        {
                            destObj.Add(s.Name, s.Value.DeepClone());
                        }
                    }
                    //マージ先(dest)のプロパティをマージ元(source)のプロパティで上書きする
                    foreach (var p in destObj.Properties())
                    {
                        var token = sourceObj.Properties().FirstOrDefault(x => x.Name == p.Name);
                        if (token != null)
                        {
                            if (token.Type == JTokenType.Null)
                            {
                                p.Value = token;
                                continue;
                            }
                            MergeToken(token, p);
                        }
                    }
                    break;
                default:
                    break;
            }

        }
    }
}