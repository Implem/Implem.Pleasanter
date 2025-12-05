// Implem.Pleasanter.Libraries.Security/GoogleRecaptchaService.cs
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.Security.Captcha
{
    public class GoogleRecaptchaService : CaptchaServiceBase
    {
        private readonly bool _isV3;
        private readonly double _scoreThreshold;

        public override string TokenFieldName => "g-recaptcha-response";
        protected override string VerificationEndpoint => "https://www.google.com/recaptcha/api/siteverify";

        public GoogleRecaptchaService(
            IHttpClientFactory httpClientFactory,
            string secretKey,
            bool isV3 = false,
            double scoreThreshold = 0.5)
            : base(httpClientFactory, secretKey)
        {
            _isV3 = isV3;
            _scoreThreshold = scoreThreshold;
        }

        protected override void PostProcessSuccess(JsonElement root, CaptchaVerificationResult result)
        {
            if (!_isV3) return;

            if (root.TryGetProperty("action", out var actionElement) && actionElement.ValueKind == JsonValueKind.String)
            {
                result.Action = actionElement.GetString();
            }

            if (root.TryGetProperty("score", out var scoreElement) && scoreElement.ValueKind == JsonValueKind.Number)
            {
                result.Score = scoreElement.GetDouble();
                if (result.Score < _scoreThreshold)
                {
                    result.ErrorType = General.Error.Types.CaptchaVerificationFailed;
                }
            }
        }
    }
}