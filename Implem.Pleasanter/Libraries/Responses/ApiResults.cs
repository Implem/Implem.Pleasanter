using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using System.Net.Http;
using System.Web.Mvc;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ApiResults
    {
        public static ContentResult Success(long id, string message)
        {
            return Get(ApiResponses.Success(id, message));
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

        public static ContentResult BadRequest(Context context)
        {
            return Get(ApiResponses.BadRequest(context: context));
        }

        public static ContentResult Unauthorized(Context context)
        {
            return Get(ApiResponses.Unauthorized(context: context));
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