using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using System.Web.Mvc;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ApiResults
    {
        public static ContentResult Success(long id, string message)
        {
            return Get(ApiResponses.Success(id, message));
        }

        public static ContentResult Error(Error.Types type)
        {
            return Get(ApiResponses.Error(type));
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

        public static ContentResult BadRequest()
        {
            return Get(ApiResponses.BadRequest());
        }

        public static ContentResult Unauthorized()
        {
            return Get(ApiResponses.Unauthorized());
        }
    }
}