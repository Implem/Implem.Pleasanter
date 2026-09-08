using Implem.DefinitionAccessor;
using System;
using System.Threading;
namespace Implem.Pleasanter.Libraries.AiConnect
{
    public static class AiConnectHttpUtilities
    {
        private const int DefaultRetryAfterSeconds = 5;
        private const int DefaultRetryAfterMaxSeconds = 60;
        private const int TooManyRequests = 429;

        public static AiConnectHttpResponse Retry429(
            Func<AiConnectHttpResponse> send,
            Action<AiConnectHttpResponse, int> onRetry = null,
            Action<AiConnectHttpResponse, int> onExceeded = null)
        {
            var response = send();
            if (response.StatusCode != TooManyRequests)
            {
                return response;
            }
            var seconds = response.RetryAfterSeconds ?? DefaultRetryAfterSeconds;
            if (Waitable(seconds: seconds) == false)
            {
                onExceeded?.Invoke(response, seconds);
                return response;
            }
            onRetry?.Invoke(response, seconds);
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            return send();
        }

        private static bool Waitable(int seconds)
        {
            return seconds >= 0
                && seconds <= MaxSeconds();
        }

        private static int MaxSeconds()
        {
            return Parameters.AiConnect?.Rag.RetryAfterMaxSeconds
                ?? DefaultRetryAfterMaxSeconds;
        }
    }
}
