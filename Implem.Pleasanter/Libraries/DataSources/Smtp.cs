using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Mails;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Implem.Pleasanter.Libraries.DataSources
{
    public class Smtp
    {
        public string Host;
        public int Port;
        public MailboxAddress From;
        public string To;
        public string Cc;
        public string Bcc;
        public string Subject;
        public string Body;
        public Context Context;

        private static readonly System.Net.Http.HttpClient HttpClient = new System.Net.Http.HttpClient();

        private const string OAuthTokenKeyPrefix = "OAuthToken";

        private const string SmtpOAuthTokenSessionGuid = "SmtpOAuthToken";

        private class OAuthTokenData
        {
            public string Token { get; set; }
            public DateTime ExpiresAtUtc { get; set; }
        }

        public Smtp(
            Context context,
            string host,
            int port,
            MailboxAddress from,
            string to,
            string cc,
            string bcc,
            string subject,
            string body)
        {
            Context = context;
            Host = host;
            Port = port;
            From = from;
            To = to;
            Cc = cc;
            Bcc = bcc;
            Subject = subject;
            Body = body;
        }

        public async void Send(Context context, Attachments attachments = null)
        {
            try
            {
                var message = new MimeMessage();
                var enc = GetEncodingOrDefault(context: context,
                    encoding: Parameters.Mail.Encoding);
                message.From.Add(Addresses.From(From).SetEncoding(enc));
                Addresses.Get(
                    context: context,
                    addresses: To)
                    .ForEach(to => message.To.Add(MailboxAddress.Parse(to).SetEncoding(enc)));
                Addresses.Get(
                    context: context,
                    addresses: Cc)
                    .ForEach(cc => message.Cc.Add(MailboxAddress.Parse(cc).SetEncoding(enc)));
                Addresses.Get(
                    context: context,
                    addresses: Bcc)
                        .ForEach(bcc => message.Bcc.Add(MailboxAddress.Parse(bcc).SetEncoding(enc)));
                message.Headers.Replace(HeaderId.Subject, enc, Subject);
                var textPart = new TextPart(MimeKit.Text.TextFormat.Plain);
                textPart.SetText(enc, Body);
                textPart.ContentTransferEncoding = GetContentEncodingForTransfer(
                    encoding: enc,
                    contentEncoding: Parameters.Mail.ContentEncoding);
                var mimeParts = attachments
                    ?.Where(attachment => attachment?.Base64?.IsNullOrEmpty() == false)
                    .Select(attachment =>
                    {
                        var fileName = attachment.Name ?? attachment.FileName ?? string.Empty;
                        var mimeType = attachment.ContentType.IsNullOrEmpty()
                            ? MimeTypes.GetMimeType(fileName)
                            : attachment.ContentType;
                        var stream = new MemoryStream(Convert.FromBase64String(attachment.Base64));
                        var mimePart = new MimePart(mimeType);
                        mimePart.FileName = Strings.CoalesceEmpty(fileName, "NoName");
                        mimePart.Content = new MimeContent(stream);
                        return mimePart;
                    }).ToList();
                if (mimeParts?.Count > 0)
                {
                    var multipart = new Multipart("mixed");
                    multipart.Add(textPart);
                    foreach (var part in mimeParts)
                    {
                        multipart.Add(part);
                    }
                    message.Body = multipart;
                }
                else
                {
                    message.Body = textPart;
                }
                using (var smtpClient = new SmtpClient())
                {
                    if (Parameters.Mail.ServerCertificateValidationCallback)
                    {
                        smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    }
                    var options = Enum.TryParse<MailKit.Security.SecureSocketOptions>(Parameters.Mail.SecureSocketOptions, out var op)
                        ? op
                        : (Parameters.Mail.SmtpEnableSsl
                            ? MailKit.Security.SecureSocketOptions.StartTls
                            : MailKit.Security.SecureSocketOptions.None);
                    await smtpClient.ConnectAsync(Host, Port, options);

                    if (Parameters.Mail.UseOAuth)
                    {
                        var accessToken = await GetAccessTokenAsync();
                        await smtpClient.AuthenticateAsync(new SaslMechanismOAuth2(Parameters.Mail.SmtpUserName, accessToken));
                    }
                    else if (!Parameters.Mail.SmtpUserName.IsNullOrEmpty()
                        && !Parameters.Mail.SmtpPassword.IsNullOrEmpty())
                    {
                        await smtpClient.AuthenticateAsync(Parameters.Mail.SmtpUserName, Parameters.Mail.SmtpPassword);
                    }

                    await smtpClient.SendAsync(message);
                    await smtpClient.DisconnectAsync(true);
                }
            }
            catch (Exception e)
            {
                new SysLogModel(Context, e);
            }
        }

        private Encoding GetEncodingOrDefault(Context context, string encoding)
        {
            if (encoding == null)
            {
                return Encoding.UTF8;
            }
            var encodingInfo = Encoding.GetEncodings()
                .FirstOrDefault(o => o.Name == encoding
                    || o.DisplayName == encoding
                    || o.CodePage.ToString() == encoding);
            if (encodingInfo == null)
            {
                new SysLogModel(
                    context: context,
                    method: nameof(GetEncodingOrDefault),
                    message: $"{encoding} is not supported Encoding. Falling back to UTF-8.",
                    errStackTrace: $"Supported Encodings are {Encoding.GetEncodings().Select(o => o.Name).Join(",")}.",
                    sysLogType: SysLogModel.SysLogTypes.Exception);
                return Encoding.UTF8;
            }
            return Encoding.GetEncoding(encodingInfo.Name);
        }

        private ContentEncoding GetContentEncodingForTransfer(Encoding encoding, ParameterAccessor.Parts.Types.ContentEncodings? contentEncoding)
        {
            if (encoding == Encoding.UTF8 || contentEncoding == null)
            {
                return ContentEncoding.Default;
            }
            if (Enum.TryParse(contentEncoding.ToString(), out ContentEncoding type)
                && Enum.IsDefined(typeof(ContentEncoding), type))
            {
                return type;
            }
            return ContentEncoding.Default;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var sessionKey = $"{OAuthTokenKeyPrefix}:{Parameters.Mail.OAuthClientId}";

            return TryGetValidTokenFromSession(sessionKey) ?? await FetchAccessTokenAsync(sessionKey);
        }

        private string TryGetValidTokenFromSession(string sessionKey)
        {
            var sessions = SessionUtilities.Get(
                context: Context,
                includeUserArea: false,
                sessionGuid: SmtpOAuthTokenSessionGuid);

            if (!sessions.TryGetValue(sessionKey, out var cachedJson) || cachedJson.IsNullOrEmpty())
            {
                return null;
            }
            try
            {
                var tokenData = JsonSerializer.Deserialize<OAuthTokenData>(cachedJson);
                return tokenData?.ExpiresAtUtc > DateTime.UtcNow.AddSeconds(Parameters.Mail.OAuthTokenRefreshBufferTime)
                    ? tokenData.Token
                    : null;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private async Task<string> FetchAccessTokenAsync(string sessionKey)
        {
            ValidateOAuthParameters();

            using var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", Parameters.Mail.OAuthClientId),
                new KeyValuePair<string, string>("client_secret", Parameters.Mail.OAuthClientSecret),
                new KeyValuePair<string, string>("scope", Parameters.Mail.OAuthScope),
                new KeyValuePair<string, string>("grant_type", Parameters.Mail.OAuthGrantType)
            });

            try
            {
                var response = await HttpClient.PostAsync(Parameters.Mail.OAuthTokenEndpoint, content);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(json);

                if (!jsonDoc.RootElement.TryGetProperty("access_token", out var tokenElement))
                {
                    throw new InvalidOperationException("access_token not found in OAuth response");
                }

                var token = tokenElement.GetString() ?? throw new InvalidOperationException("access_token is null");
                var expiresIn = jsonDoc.RootElement.TryGetProperty("expires_in", out var expiresInElement)
                    ? expiresInElement.GetInt32()
                    : Parameters.Mail.OAuthDefaultExpiresIn;
                var expiresAt = DateTime.UtcNow.AddSeconds(expiresIn);

                var tokenData = new OAuthTokenData
                {
                    Token = token,
                    ExpiresAtUtc = expiresAt
                };
                SessionUtilities.Set(
                    context: Context,
                    key: sessionKey,
                    value: JsonSerializer.Serialize(tokenData),
                    sessionGuid: SmtpOAuthTokenSessionGuid);

                return token;
            }
            catch (HttpRequestException ex)
            {
                new SysLogModel(
                    context: Context,
                    e: ex,
                    extendedErrorMessage: $"OAuth token acquisition failed. Status: {ex.StatusCode}");
                throw;
            }
            catch (JsonException ex)
            {
                new SysLogModel(
                    context: Context,
                    e: ex,
                    extendedErrorMessage: "OAuth response JSON parsing failed");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                new SysLogModel(
                    context: Context,
                    e: ex,
                    extendedErrorMessage: "OAuth response validation failed");
                throw;
            }
        }

        private void ValidateOAuthParameters()
        {
            List<string> missingParams = [];

            if (Parameters.Mail.OAuthClientId.IsNullOrEmpty())
            {
                missingParams.Add(nameof(Parameters.Mail.OAuthClientId));
            }
            if (Parameters.Mail.OAuthClientSecret.IsNullOrEmpty())
            {
                missingParams.Add(nameof(Parameters.Mail.OAuthClientSecret));
            }
            if (Parameters.Mail.OAuthScope.IsNullOrEmpty())
            {
                missingParams.Add(nameof(Parameters.Mail.OAuthScope));
            }
            if (Parameters.Mail.OAuthGrantType.IsNullOrEmpty())
            {
                missingParams.Add(nameof(Parameters.Mail.OAuthGrantType));
            }
            if (Parameters.Mail.OAuthTokenEndpoint.IsNullOrEmpty())
            {
                missingParams.Add(nameof(Parameters.Mail.OAuthTokenEndpoint));
            }
            else if (!Uri.TryCreate(Parameters.Mail.OAuthTokenEndpoint, UriKind.Absolute, out var uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                missingParams.Add($"{nameof(Parameters.Mail.OAuthTokenEndpoint)} (invalid URL format)");
            }

            if (missingParams.Count > 0)
            {
                throw new InvalidOperationException(
                    $"OAuth parameters are not configured: {string.Join(", ", missingParams)}");
            }
        }
    }
}