using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.Security.Captcha
{
    public interface ICaptchaService
    {
        Task<CaptchaVerificationResult> VerifyAsync(string token, string remoteIp = null);
        string TokenFieldName { get; }
    }

    public class CaptchaVerificationResult
    {
        public double? Score { get; set; }
        public string Action { get; set; }
        public General.Error.Types ErrorType { get; set; } = General.Error.Types.None;
        public bool Success => ErrorType == General.Error.Types.None;

        private readonly List<string> _errorCodes = new();
        public IReadOnlyList<string> ErrorCodes => _errorCodes;
        internal void AddErrorCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                _errorCodes.Add(code);
            }
        }

    }
}