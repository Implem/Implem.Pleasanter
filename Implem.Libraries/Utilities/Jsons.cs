using Newtonsoft.Json;
namespace Implem.Libraries.Utilities
{
    public static class Jsons
    {
        public static string ToJson(this object obj)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
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
    }
}
