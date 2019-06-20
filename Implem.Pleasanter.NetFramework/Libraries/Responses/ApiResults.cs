using Implem.Pleasanter.Libraries.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Implem.Pleasanter.NetFramework.Libraries.Responses
{
    public static class ApiResults
    {
        public static HttpResponseMessage ToHttpResponse(this ContentResult self, HttpRequestMessage request)
        {
            var content = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse>(self.Content);
            var response = request.CreateResponse((System.Net.HttpStatusCode)content.StatusCode);
            response.Content = new StringContent(self.Content, self.ContentEncoding, self.ContentType);
            return response;
        }
    }
}