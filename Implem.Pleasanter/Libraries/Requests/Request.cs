using Implem.Libraries.Utilities;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class Request
    {
        public HttpRequest HttpRequest;

        public Request(HttpContext httpContext)
        {
            HttpRequest = httpContext != null && httpContext.User != null
                ? httpContext.Request
                : null;
        }

        public static bool IsAjax()
        {
            return new HttpRequestWrapper(HttpContext.Current.Request).IsAjaxRequest();
        }

        public string ProcessedRequestData()
        {
            return HttpRequest?.Form.ToString().Split('&')
                .Where(o => o.Contains('='))
                .Select(o => ProcessedRequestData(o))
                .Join("&");
        }

        private string ProcessedRequestData(string requestData)
        {
            switch (requestData.Substring(0, requestData.IndexOf("=")).ToLower())
            {
                case "users_password": return "Users_Password=*";
                case "users_changedpassword": return "Users_ChangedPassword=*";
                case "users_afterresetpassword": return "Users_AfterResetPassword=*";
                default: return requestData;
            }
        }

        public string HttpMethod()
        {
            return HttpRequest != null
                ? HttpRequest.HttpMethod
                : null;
        }

        public string Url()
        {
            return HttpRequest != null
                ? HttpRequest.Url.ToString()
                : null;
        }

        public string UrlReferrer()
        {
            return HttpRequest.UrlReferrer != null
                ? HttpRequest.UrlReferrer.ToString()
                : null;
        }

        public string UserHostName()
        {
            return HttpRequest.UserHostName != null
                ? HttpRequest.UserHostName
                : null;
        }

        public string UserHostAddress()
        {
            return HttpRequest.UserHostAddress != null
                ? HttpRequest.UserHostAddress
                : null;
        }

        public string UserLanguage()
        {
            return HttpRequest.UserLanguages != null && HttpRequest.UserLanguages.Length > 0
                ? HttpRequest.UserLanguages[0]
                : null;
        }

        public string UserAgent()
        {
            return HttpRequest.UserAgent != null
                ? HttpRequest.UserAgent
                : null;
        }
    }
}