using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Implem.Pleasanter.Libraries.Requests;

namespace Implem.Pleasanter.Libraries.Security.Captcha
{
    public interface ICaptchaVerificationService
    {
        Task<CaptchaVerificationResult> VerifyAsync(Context context);
    }

    public class CaptchaVerificationService : ICaptchaVerificationService
    {
        private readonly ICaptchaServiceFactory _captchaServiceFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CaptchaVerificationService(
            ICaptchaServiceFactory captchaServiceFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _captchaServiceFactory = captchaServiceFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CaptchaVerificationResult> VerifyAsync(Context context)
        {
            var captchaService = _captchaServiceFactory.GetCaptchaService();
            if (captchaService == null)
            {
                return new CaptchaVerificationResult();
            }

            var httpContext = _httpContextAccessor.HttpContext;
            var token = httpContext?.Request.Form[captchaService.TokenFieldName].ToString();
            var remoteIp = httpContext?.Connection.RemoteIpAddress?.ToString();

            var result = await captchaService.VerifyAsync(token, remoteIp);

            return result;
        }
    }
}