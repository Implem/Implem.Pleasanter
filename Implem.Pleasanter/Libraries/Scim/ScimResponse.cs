using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Implem.Pleasanter.Libraries.Scim
{
    public static class ScimResponse
    {
        private static readonly JsonSerializerSettings Settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static ContentResult Content(object body, int statusCode = StatusCodes.Status200OK)
        {
            return new ContentResult
            {
                Content = body == null
                    ? string.Empty
                    : JsonConvert.SerializeObject(body, Formatting.None, Settings),
                ContentType = "application/scim+json; charset=utf-8",
                StatusCode = statusCode
            };
        }

        public static ContentResult NoContent()
        {
            return new ContentResult
            {
                Content = string.Empty,
                StatusCode = StatusCodes.Status204NoContent
            };
        }

        public static ContentResult Error(
            int statusCode,
            string detail,
            string scimType = null)
        {
            return Content(
                body: new ScimError
                {
                    Status = statusCode.ToString(),
                    ScimType = scimType,
                    Detail = detail
                },
                statusCode: statusCode);
        }
    }

    public class ScimServiceResult
    {
        public int StatusCode { get; set; } = StatusCodes.Status200OK;
        public object Body { get; set; }

        public ContentResult ToContent()
        {
            return StatusCode == StatusCodes.Status204NoContent
                ? ScimResponse.NoContent()
                : ScimResponse.Content(Body, StatusCode);
        }
    }
}
