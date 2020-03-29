using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ApiResults
    {
        public static ContentResult Success(long id, string message, int? limitPerDate = null, int? limitRemaining = null)
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

        public static ContentResult Error(Context context, ErrorData errorData, params string[] data)
        {
            return Get(ApiResponses.Error(
                context: context,
                errorData: errorData,
                data: data));
        }

        public static ContentResult Get(ApiResponse apiResponse)
        {
            return Get(apiResponse.ToJson());
        }

        public static ContentResult Get(string apiResponse)
        {
            return new ContentResult
            {
                ContentType = "application/json",
                Content = apiResponse
            };
        }

        public static ContentResult Get(int statusCode, int limitPerDate, int limitRemaining, object response)
        {
            return new ContentResult
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

        public static ContentResult BadRequest(Context context)
        {
            return Get(ApiResponses.BadRequest(context: context));
        }

        public static ContentResult Unauthorized(Context context)
        {
            return Get(ApiResponses.Unauthorized(context: context));
        }

        public static ContentResult Forbidden(Context context)
        {
            return Get(ApiResponses.Forbidden(context: context));
        }

        public static ContentResult OverTenantStorageSize(Context context, decimal? maxSize)
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