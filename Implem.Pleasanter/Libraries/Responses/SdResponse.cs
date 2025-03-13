using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Newtonsoft.Json;
using System;
using System.Linq;

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
            string referrer = url.Split('?')
                .First()
                .Split('/')
                .Last();          
            switch (referrer)
            {
                case "new":
                case "edit":
                case "index":
                    return url;
                default:
                    int number;
                    bool isNumeric = int.TryParse(referrer, out number);
                    if (isNumeric)
                    {
                        return url;
                    }
                    return url.Replace(referrer, "index");
            }
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}