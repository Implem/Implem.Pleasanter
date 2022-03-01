using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using System.Net.Http;
using System.Web.Mvc;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ApiResults
    {
        public static ContentResultInheritance Success(long id, string message, int? limitPerDate = null, int? limitRemaining = null)
        {
            if (limitPerDate == 0)
            {
                limitPerDate = null;
                limitRemaining = null;
            }
            return Get(ApiResponses.Success(
                id: id,
                message: message,
                limitPerDate: limitPerDate,
                limitRemaining: limitRemaining));
        }

        public static ContentResultInheritance Error(Context context, ErrorData errorData, params string[] data)
        {
            return Get(ApiResponses.Error(
                context: context,
                errorData: errorData,
                data: data));
        }

        public static ContentResultInheritance Get(ApiResponse apiResponse)
        {
            return Get(apiResponse.ToJson());
        }

        public static ContentResultInheritance Get(string apiResponse)
        {
            return new ContentResultInheritance
            {
                ContentType = "application/json",
                Content = apiResponse
            };
        }

        public static ContentResultInheritance Get(
            int statusCode,
            int limitPerDate,
            int limitRemaining,
            object response)
        {
            return new ContentResultInheritance
            {
                ContentType = "application/json",
                Content = new
                {
                    StatusCode = statusCode,
                    LimitPerDate = limitPerDate == 0
                        ? null
                        : (int?)limitPerDate,
                    LimitRemaining = limitPerDate == 0
                        ? null
                        : (int?)limitRemaining,
                    Response = response
                }.ToJson()
            };
        }

        public static ContentResultInheritance BadRequest(Context context)
        {
            return Get(ApiResponses.BadRequest(context: context));
        }

        public static ContentResultInheritance Unauthorized(Context context)
        {
            return Get(ApiResponses.Unauthorized(context: context));
        }

        public static ContentResultInheritance Forbidden(Context context)
        {
            return Get(ApiResponses.Forbidden(context: context));
        }

        public static ContentResultInheritance NotFound(Context context)
        {
            return Get(ApiResponses.NotFound(context: context));
        }

        public static ContentResultInheritance OverTenantStorageSize(Context context, decimal? maxSize)
        {
            var result = Get(ApiResponses.OverTenantStorageSize(
                context: context,
                maxSize: maxSize));
            return result;
        }

        public static HttpResponseMessage ToHttpResponse(this ContentResult self, HttpRequestMessage request)
        {
            var content = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse>(self.Content);
            var response = request.CreateResponse((System.Net.HttpStatusCode)content.StatusCode);
            response.Content = new StringContent(self.Content, self.ContentEncoding, self.ContentType);
            return response;
        }
    }
}