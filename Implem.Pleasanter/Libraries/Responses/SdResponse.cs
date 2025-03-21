using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            if (referrer is "new" or "edit" or "index")
            {
                return url;
            }
            return int.TryParse(referrer, out _) ? url : url.Replace(referrer, "index");
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static SdResponse SdResponseJson(
            ResponseCollection response)
        {
            response.Select(o =>
            {
                if (o.Method == "Messages")
                {
                    var value = o.Value as Dictionary<string, object>;
                    if (value != null && value.ContainsKey("Id") && value.ContainsKey("Text"))
                    {
                        return new SdResponse(
                            method: value["Id"].ToString(),
                            value: value["Text"]);
                    }
                }
                return null;
            });
            return null;
        }
    }
}