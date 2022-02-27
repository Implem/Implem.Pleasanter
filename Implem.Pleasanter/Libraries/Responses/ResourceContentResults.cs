using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ResourceContentResults
    {
        public static ContentResultInheritance ToRecourceContentResult(this ContentResult self, HttpRequest request)
        {
            var response = new ContentResultInheritance();
            response.Content = self.Content;
            response.ContentType = self.ContentType;
            return response;
        }
    }
}
