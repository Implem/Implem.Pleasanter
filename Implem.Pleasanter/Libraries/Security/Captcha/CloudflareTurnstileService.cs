using System.Net.Http;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.Security.Captcha
{
    public class CloudflareTurnstileService : CaptchaServiceBase
    {
        public override string TokenFieldName => "cf-turnstile-response";
        protected override string VerificationEndpoint => "https://challenges.cloudflare.com/turnstile/v0/siteverify";

        public CloudflareTurnstileService(
            IHttpClientFactory httpClientFactory,
            string secretKey)
            : base(httpClientFactory, secretKey)
        {
        }

        // Cloudflare 固有の追加処理があれば PostProcessSuccess / AddExtraFormFields をオーバーライド
        public new Task<CaptchaVerificationResult> VerifyAsync(string token, string remoteIp = null)
            => base.VerifyAsync(token, remoteIp);
    }
}