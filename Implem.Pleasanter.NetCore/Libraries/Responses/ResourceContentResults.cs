using Microsoft.AspNetCore.Http;
using System.Web.Mvc;
namespace Implem.Pleasanter.NetCore.Libraries.Responses
{
    public static class ResourceContentResults
    {
        public static Microsoft.AspNetCore.Mvc.ContentResult ToRecourceContentResult(this ContentResult self, HttpRequest request)
        {
            var response = new Microsoft.AspNetCore.Mvc.ContentResult();
            response.Content = self.Content;
            response.ContentType = self.ContentType;
            return response;
        }
    }
}
