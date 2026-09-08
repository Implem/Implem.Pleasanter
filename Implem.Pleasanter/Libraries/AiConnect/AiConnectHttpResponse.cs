using System;
namespace Implem.Pleasanter.Libraries.AiConnect
{
    public class AiConnectHttpResponse
    {
        public int StatusCode { get; set; }
        public string Body { get; set; } = string.Empty;
        public Exception Exception { get; set; }
        public int? RetryAfterSeconds { get; set; }

        public bool Succeeded
        {
            get
            {
                return StatusCode >= 200 && StatusCode <= 299;
            }
        }
    }
}