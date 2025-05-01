using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            return Get(apiResponse.ToJson(), apiResponse.StatusCode);
        }

        public static ContentResultInheritance Get(string apiResponse, int? statusCode = null)
        {
            return new ContentResultInheritance
            {
                ContentType = "application/json;charset=utf-8",
                Content = apiResponse,
                StatusCode = statusCode
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
                ContentType = "application/json;charset=utf-8",
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

        public static ContentResultInheritance Duplicated(Context context, string message)
        {
            return Get(ApiResponses.Duplicated(
                context: context,
                message: message));
        }

        public static ContentResultInheritance ToHttpResponse(this ContentResultInheritance self, HttpRequest request)
        {
            var response = new ContentResultInheritance();
            var content = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResponse>(self.Content);
            response.StatusCode = content.StatusCode;
            response.Content = self.Content;
            response.ContentType = self.ContentType;
            return response;
        }
    }
}