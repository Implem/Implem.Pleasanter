using Implem.ParameterAccessor.Parts;
using System.Net.Http;
using Implem.DefinitionAccessor;

namespace Implem.Pleasanter.Libraries.Security.Captcha
{
    public interface ICaptchaServiceFactory
    {
        ICaptchaService GetCaptchaService();
    }
    public class CaptchaServiceFactory : ICaptchaServiceFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private ICaptchaService _cachedService;
        private CaptchaConfig _cachedConfig;
        private readonly object _lockObject = new object();
        const double RECAPTCHAV3_DEFAULT_SSCORE_THRESHOLD = 0.5; // デフォルトのスコア閾値

        public CaptchaServiceFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public ICaptchaService GetCaptchaService()
        {
            var currentConfig = Parameters.Security.CaptchaConfig;

            lock (_lockObject)
            {
                if (IsSameConfiguration(_cachedConfig, currentConfig) && _cachedService != null)
                {
                    return _cachedService;
                }

                _cachedConfig = currentConfig;
                _cachedService = CreateCaptchaService(currentConfig);

                return _cachedService;
            }
        }

        private bool IsSameConfiguration(CaptchaConfig cached, CaptchaConfig current)
        {
            if (cached == null || current == null)
                return cached == current;

            return cached.Type == current.Type &&
                   cached.SecretKey == current.SecretKey &&
                   cached.SiteKey == current.SiteKey &&
                   (cached.RecaptchaV3?.DefaultScoreThreshold ?? RECAPTCHAV3_DEFAULT_SSCORE_THRESHOLD) ==
                   (current.RecaptchaV3?.DefaultScoreThreshold ?? RECAPTCHAV3_DEFAULT_SSCORE_THRESHOLD);
        }

        private ICaptchaService CreateCaptchaService(CaptchaConfig captcha)
        {
            if (captcha == null || captcha.Type == CaptchaConfig.Types.None)
            {
                return null; // NullCaptchaServiceの代わりにnullを返す
            }

            return captcha.Type switch
            {
                CaptchaConfig.Types.RecaptchaV3 => new GoogleRecaptchaService(
                    _httpClientFactory,
                    captcha.SecretKey,
                    isV3: true,
                    scoreThreshold: captcha.RecaptchaV3?.DefaultScoreThreshold ?? RECAPTCHAV3_DEFAULT_SSCORE_THRESHOLD),

                CaptchaConfig.Types.ReCaptchaV2 => new GoogleRecaptchaService(
                    _httpClientFactory,
                    captcha.SecretKey,
                    isV3: false),

                CaptchaConfig.Types.Turnstile => new CloudflareTurnstileService(
                    _httpClientFactory,
                    captcha.SecretKey),

                _ => null
            };
        }
    }
}