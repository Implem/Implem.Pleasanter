using System;
namespace Implem.Pleasanter.Libraries.Responses
{
    [Serializable]
    public class ApiResponse
    {
        public long Id;
        public int StatusCode;
        public string Message;

        public ApiResponse(long id, int statusCode, string message)
        {
            Id = id;
            StatusCode = statusCode;
            Message = message;
        }

        public ApiResponse(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}