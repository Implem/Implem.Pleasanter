using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Implem.PleasanterSetup
{
    public static class Jsons
    {
        public static string ToJson(
            this object obj,
            bool extendedColumns = false,
            Formatting formatting = Formatting.Indented)
        {
            var settings = new JsonSerializerSettings();
            settings.Formatting = formatting;
            if (extendedColumns)
            {
                settings.NullValueHandling = NullValueHandling.Ignore;
            }
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static T Deserialize<T>(
            this string str)
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
    }
}