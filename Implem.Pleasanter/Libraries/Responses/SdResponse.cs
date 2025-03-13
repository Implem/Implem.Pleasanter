using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Newtonsoft.Json;

namespace Implem.Pleasanter.Libraries.Responses
{
    public class SdResponse
    {
        public string Method;
        public object Value;
        public string Url;
        public SdResponse(
            string method,
            object value,
            string url)
        {
            Method = method;
            Value = value;
            Url = Href(url);
        }

        public SdResponse(
            string method,
            string url)
        {
            Method = method;
            Value = null;
            Url = Href(url);
        }

        public SdResponse(
            string method,
            object value)
        {
            Method = method;
            Value = value;
            Url = null;
        }


        public string Href(string url)
        {
            string referrer = url.Substring(url.LastIndexOf('/') + 1).ToLower();
            switch (referrer)
            {
                //ItemNewだと完全なUrlが取得できない
                case "new":
                case "edit":
                case "index":
                    return url + "?sds=1";
                default:
                    //url.Replace(referrer, "index");
                    return url.Replace(referrer, "index") + "?sds=1";
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static string Updated(Context context, params string[] data)
        {
            return Displays.Updated(
                    context: context,
                    data: data);
        }

    }
}