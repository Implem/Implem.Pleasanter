using System;
namespace Implem.Pleasanter.Libraries.Responses
{
    [Serializable]
    public class ApiResponse
    {
        public long Id;
        public int StatusCode;
        public int? LimitPerDate;
        public int? LimitRemaining;
        public string Message;
        

        public ApiResponse()
        {
        }

        public ApiResponse(long id, int statusCode, string message, int? limitPerDate = null, int? limitRemaining = null)
        {
            Id = id;
            StatusCode = statusCode;
            Message = message;
            LimitPerDate = limitPerDate;
            LimitRemaining = limitRemaining;
        }

        public ApiResponse(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}