using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelHttpClient
    {
        private static readonly HttpClient _httpClient;
        private readonly CookieContainer _cookieContainer;
        public string RequestUri { get; set; }
        public string Content { get; set; }
        public string Encoding { get; set; } = "utf-8";
        public string MediaType { get; set; } = "application/json";
        public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, IList<string>> ResponseHeaders { get; set; } = new Dictionary<string, IList<string>>();
        public int TimeOut { get; set; } = Parameters.Script.ServerScriptHttpClientTimeOut;
        public int StatusCode { get; private set; }
        public bool IsSuccess { get; private set; }
        public bool IsTimeOut { get; private set; }

        static ServerScriptModelHttpClient()
        {
            _httpClient = new HttpClient(new HttpClientHandler
            {
                UseCookies = false,
                AllowAutoRedirect = false
            })
            {
                Timeout = Timeout.InfiniteTimeSpan
            };
        }

        public ServerScriptModelHttpClient(Context context)
        {
            context.ServerScriptCookieContainer ??= new CookieContainer();
            _cookieContainer = context.ServerScriptCookieContainer;
        }

        public string Get() => Core(HttpMethod.Get);

        public string Post() => Core(HttpMethod.Post, withContent: true);

        public string Put() => Core(HttpMethod.Put, withContent: true);

        public string Patch() => Core(HttpMethod.Patch, withContent: true);

        public string Delete() => Core(HttpMethod.Delete);

        private string Core(HttpMethod method, bool withContent = false)
        {
            try
            {
                ResponseHeaders.Clear();
                StatusCode = default;
                IsSuccess = false;
                IsTimeOut = false;
                using var cts = new CancellationTokenSource();
                cts.CancelAfter(GetTimeOut());
                var originUri = new Uri(RequestUri);
                var requestUri = originUri;
                var requestMethod = method;
                var hasContent = withContent;
                for (var redirectCount = 0; ; redirectCount++)
                {
                    using var request = CreateHttpRequest(
                        method: requestMethod,
                        content: hasContent ? CreateContent() : null,
                        requestUri: requestUri,
                        originUri: originUri,
                        redirected: redirectCount > 0);
                    HttpResponseMessage response;
                    try
                    {
                        response = _httpClient.SendAsync(request, cts.Token).Result;
                    }
                    catch (OperationCanceledException ex) when (ex.CancellationToken == cts.Token)
                    {
                        IsTimeOut = true;
                        return default;
                    }
                    using (response)
                    {
                        StoreCookiesFromResponse(response, requestUri);
                        var redirectUri = GetRedirectUri(
                            response: response,
                            requestUri: requestUri,
                            redirectCount: redirectCount);
                        if (redirectUri == null)
                        {
                            StatusCode = (int)response.StatusCode;
                            IsSuccess = response.IsSuccessStatusCode;
                            foreach (var header in response.Headers)
                            {
                                ResponseHeaders.Add(header.Key, header.Value.ToArray());
                            }
                            var responseContent = response.Content.ReadAsStringAsync().Result;
                            return responseContent;
                        }
                        if (ChangesToGet(requestMethod, response.StatusCode))
                        {
                            requestMethod = HttpMethod.Get;
                            hasContent = false;
                        }
                        requestUri = redirectUri;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static Uri GetRedirectUri(
            HttpResponseMessage response,
            Uri requestUri,
            int redirectCount)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.MultipleChoices:
                case HttpStatusCode.MovedPermanently:
                case HttpStatusCode.Found:
                case HttpStatusCode.SeeOther:
                case HttpStatusCode.TemporaryRedirect:
                case HttpStatusCode.PermanentRedirect:
                    break;
                default:
                    return null;
            }
            var location = response.Headers.Location;
            var maxRedirectCount = Parameters.Script.ServerScriptHttpClientMaxRedirectCount;
            if (location == null || redirectCount >= maxRedirectCount)
            {
                return null;
            }
            var redirectUri = new Uri(requestUri, location);
            if (redirectUri.Scheme != Uri.UriSchemeHttp && redirectUri.Scheme != Uri.UriSchemeHttps)
            {
                return null;
            }
            if (requestUri.Scheme == Uri.UriSchemeHttps && redirectUri.Scheme == Uri.UriSchemeHttp)
            {
                return null;
            }
            return redirectUri;
        }

        private static bool ChangesToGet(HttpMethod method, HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case HttpStatusCode.MultipleChoices:
                case HttpStatusCode.MovedPermanently:
                case HttpStatusCode.Found:
                    return method == HttpMethod.Post;
                case HttpStatusCode.SeeOther:
                    return method != HttpMethod.Get && method != HttpMethod.Head;
                default:
                    return false;
            }
        }

        private static bool IsHeader(string name, string headerName)
        {
            return string.Equals(name, headerName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSameOrigin(Uri uri, Uri originUri)
        {
            return string.Equals(uri.Host, originUri.Host, StringComparison.OrdinalIgnoreCase)
                && uri.Port == originUri.Port;
        }

        private HttpRequestMessage CreateHttpRequest(
            HttpMethod method,
            HttpContent content,
            Uri requestUri,
            Uri originUri,
            bool redirected)
        {
            var request = new HttpRequestMessage()
            {
                Method = method,
                RequestUri = requestUri,
                Content = content
            };
            var sameOrigin = IsSameOrigin(requestUri, originUri);
            foreach (var header in RequestHeaders)
            {
                if ((redirected && IsHeader(header.Key, "Authorization"))
                    || (!sameOrigin && IsHeader(header.Key, "Cookie")))
                {
                    continue;
                }
                request.Headers.Add(header.Key, header.Value);
            }
            var cookieHeader = _cookieContainer.GetCookieHeader(requestUri);
            if (!cookieHeader.IsNullOrEmpty())
            {
                request.Headers.Add("Cookie", cookieHeader);
            }
            return request;
        }

        public void ClearCookies()
        {
            foreach (Cookie cookie in _cookieContainer.GetAllCookies())
            {
                cookie.Expired = true;
            }
        }

        public void ClearCookies(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return;
            }
            foreach (Cookie cookie in _cookieContainer.GetAllCookies())
            {
                if (!TryCreateProbeUri(uri.Host, cookie.Path, out var probeUri))
                {
                    continue;
                }
                foreach (Cookie sent in _cookieContainer.GetCookies(probeUri))
                {
                    if (sent.Name == cookie.Name
                        && sent.Domain == cookie.Domain
                        && sent.Path == cookie.Path)
                    {
                        cookie.Expired = true;
                        break;
                    }
                }
            }
        }

        private static bool TryCreateProbeUri(string host, string cookiePath, out Uri probeUri)
        {
            var path = (cookiePath ?? string.Empty).Trim('"');
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            return Uri.TryCreate(
                uriString: $"{Uri.UriSchemeHttps}://{host}{path}",
                uriKind: UriKind.Absolute,
                result: out probeUri);
        }

        private void StoreCookiesFromResponse(HttpResponseMessage response, Uri requestUri)
        {
            if (response.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders))
            {
                foreach (var setCookie in setCookieHeaders)
                {
                    try
                    {
                        _cookieContainer.SetCookies(requestUri, setCookie);
                    }
                    catch (CookieException)
                    {
                    }
                }
            }
        }

        private HttpContent CreateContent()
        {
            if (Content.IsNullOrEmpty())
            {
                return null;
            }
            return new StringContent(
                content: Content,
                encoding: System.Text.Encoding.GetEncoding(Encoding),
                mediaType: MediaType);
        }

        private TimeSpan GetTimeOut()
        {
            var timeOut = TimeSpan.FromMilliseconds(TimeOut);
            var timeOutMax = TimeSpan.FromMilliseconds(Parameters.Script.ServerScriptHttpClientTimeOutMax);
            var timeOutMin = TimeSpan.FromMilliseconds(Parameters.Script.ServerScriptHttpClientTimeOutMin);

            timeOut = timeOut.Between(timeOutMin, timeOutMax)
                ? timeOut
                : TimeSpan.FromSeconds(100);

            return timeOut == TimeSpan.Zero ? Timeout.InfiniteTimeSpan : timeOut;
        }
    }
}
