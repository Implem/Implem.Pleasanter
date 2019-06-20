using Implem.Pleasanter.Libraries.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Implem.Pleasanter.NetCore.Libraries.Responses
{
    public static class ApiResults
    {
        public static Microsoft.AspNetCore.Mvc.ContentResult ToHttpResponse(this ContentResult self, HttpRequest request)
        {
            var response = new Microsoft.AspNetCore.Mvc.ContentResult();
            var content = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse>(self.Content);
            response.StatusCode = content.StatusCode;
            response.Content =self.Content;
            response.ContentType = self.ContentType;
            return response;
        }
    }
}
