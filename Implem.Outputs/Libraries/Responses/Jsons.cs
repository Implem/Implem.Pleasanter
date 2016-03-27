using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class Jsons
    {
        public static JsonResult Json(this Dictionary<string, object> result)
        {
            return Json(result);
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T Deserialize<T>(this string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
