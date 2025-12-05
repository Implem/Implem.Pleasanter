using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.Security.Captcha
{
    /// <summary>
    /// CAPTCHAサービスの共通基底クラス。
    /// </summary>
    public abstract class CaptchaServiceBase : ICaptchaService
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly string _secretKey;

        protected CaptchaServiceBase(IHttpClientFactory httpClientFactory, string secretKey)
        {
            _httpClientFactory = httpClientFactory;
            _secretKey = secretKey;
        }

        public abstract string TokenFieldName { get; }
        protected abstract string VerificationEndpoint { get; }

        public async Task<CaptchaVerificationResult> VerifyAsync(string token, string remoteIp = null)
        {
            var result = new CaptchaVerificationResult();

            if (string.IsNullOrEmpty(token))
            {
                result.ErrorType = General.Error.Types.CaptchaVerificationFailed;
                return result;
            }

            try
            {
                using var httpClient = _httpClientFactory.CreateClient();

                var formData = new Dictionary<string, string>
                {
                    ["secret"] = _secretKey,
                    ["response"] = token
                };

                if (!string.IsNullOrEmpty(remoteIp))
                {
                    formData["remoteip"] = remoteIp;
                }

                AddExtraFormFields(formData);

                using var content = new FormUrlEncodedContent(formData);
                var response = await httpClient.PostAsync(VerificationEndpoint, content).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    result.ErrorType = General.Error.Types.CaptchaVerificationFailed;
                    return result;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                using var doc = JsonDocument.Parse(jsonResponse);
                var root = doc.RootElement;

                var captchaSuccess = root.TryGetProperty("success", out var successElement) &&
                                     successElement.ValueKind == JsonValueKind.True;

                if (!captchaSuccess)
                {
                    ExtractErrorCodesIfAny(root, result);
                    result.ErrorType = General.Error.Types.CaptchaVerificationFailed;
                    PostProcessFailure(root, result);
                    return result;
                }

                PostProcessSuccess(root, result);
                return result;
            }
            catch (HttpRequestException)
            {
                result.ErrorType = General.Error.Types.CaptchaVerificationFailed;
                return result;
            }
            catch (JsonException)
            {
                result.ErrorType = General.Error.Types.CaptchaVerificationFailed;
                return result;
            }
            catch (Exception)
            {
                result.ErrorType = General.Error.Types.CaptchaVerificationFailed;
                return result;
            }
        }

        protected virtual void AddExtraFormFields(Dictionary<string, string> formData) { }
        protected virtual void PostProcessSuccess(JsonElement root, CaptchaVerificationResult result) { }
        protected virtual void PostProcessFailure(JsonElement root, CaptchaVerificationResult result) { }

        private static void ExtractErrorCodesIfAny(JsonElement root, CaptchaVerificationResult result)
        {
            if (root.TryGetProperty("error-codes", out var errorCodesElement) &&
                errorCodesElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var errorCode in errorCodesElement.EnumerateArray())
                {
                    var code = errorCode.GetString();
                    if (!string.IsNullOrEmpty(code))
                    {
                        result.AddErrorCode(code);
                    }
                }
            }
        }
    }
}