using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;
namespace Implem.Libraries.Utilities
{
    public static class Jsons
    {
        public static JsonResult Json(this Dictionary<string, object> result)
        {
            return Json(result);
        }

        public static string ToJson(this object obj)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static T Deserialize<T>(this string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
