using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Mails;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using MailKit.Net.Proxy;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
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
            From = Addresses.From(from);
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
                var enc = MailEncodings.GetEncodingOrDefault(context: context,
                    encoding: Parameters.Mail.Encoding);
                message.From.Add(From.SetEncoding(enc));
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
                textPart.ContentTransferEncoding = MailEncodings.GetContentEncodingForTransfer(
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
                        mimePart.ContentTransferEncoding = ContentEncoding.Base64;
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
                    if (Parameters.Mail.SmtpTimeout > 0)
                    {
                        smtpClient.Timeout = Parameters.Mail.SmtpTimeout;
                    }
                    smtpClient.CheckCertificateRevocation = Parameters.Mail.SmtpCheckCertificateRevocation;
                    smtpClient.RequireTLS = Parameters.Mail.SmtpRequireTls;
                    if (!Parameters.Mail.SmtpLocalDomain.IsNullOrEmpty())
                    {
                        smtpClient.LocalDomain = Parameters.Mail.SmtpLocalDomain;
                    }
                    if (!Parameters.Mail.SmtpSslProtocols.IsNullOrEmpty())
                    {
                        if (Enum.TryParse<SslProtocols>(Parameters.Mail.SmtpSslProtocols, out var sslProtocols))
                        {
                            smtpClient.SslProtocols = sslProtocols;
                        }
                        else
                        {
                            new SysLogModel(
                                context: context,
                                method: nameof(Send),
                                message: $"Invalid SmtpSslProtocols: {Parameters.Mail.SmtpSslProtocols}",
                                sysLogType: SysLogModel.SysLogTypes.Warning);
                        }
                    }
                    if (!Parameters.Mail.SmtpLocalEndPoint.IsNullOrEmpty())
                    {
                        if (IPEndPoint.TryParse(Parameters.Mail.SmtpLocalEndPoint, out var localEndPoint))
                        {
                            smtpClient.LocalEndPoint = localEndPoint;
                        }
                        else
                        {
                            new SysLogModel(
                                context: context,
                                method: nameof(Send),
                                message: $"Invalid SmtpLocalEndPoint: {Parameters.Mail.SmtpLocalEndPoint}",
                                sysLogType: SysLogModel.SysLogTypes.Warning);
                        }
                    }
                    if (!Parameters.Mail.SmtpProxyType.IsNullOrEmpty()
                        && !Parameters.Mail.SmtpProxyHost.IsNullOrEmpty()
                        && Parameters.Mail.SmtpProxyPort > 0)
                    {
                        smtpClient.ProxyClient = CreateProxyClient(
                            context: context,
                            proxyType: Parameters.Mail.SmtpProxyType,
                            host: Parameters.Mail.SmtpProxyHost,
                            port: Parameters.Mail.SmtpProxyPort,
                            userName: Parameters.Mail.SmtpProxyUserName,
                            password: Parameters.Mail.SmtpProxyPassword,
                            serverCertificateValidationCallback: Parameters.Mail.SmtpProxyServerCertificateValidationCallback,
                            checkCertificateRevocation: Parameters.Mail.SmtpProxyCheckCertificateRevocation,
                            sslProtocols: Parameters.Mail.SmtpProxySslProtocols,
                            clientCertificate: Parameters.Mail.SmtpProxyClientCertificate);
                    }
                    if (Parameters.Mail.SmtpClientCertificate != null
                        && !Parameters.Mail.SmtpClientCertificate.FindValue.IsNullOrEmpty())
                    {
                        smtpClient.ClientCertificates = GetClientCertificates(Parameters.Mail.SmtpClientCertificate);
                    }
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

        private static IProxyClient CreateProxyClient(
            Context context,
            string proxyType,
            string host,
            int port,
            string userName,
            string password,
            bool serverCertificateValidationCallback,
            bool checkCertificateRevocation,
            string sslProtocols,
            SigninCertificate clientCertificate)
        {
            var hasCredentials = !userName.IsNullOrEmpty()
                && !password.IsNullOrEmpty();
            switch (proxyType.ToLowerInvariant())
            {
                case "socks4":
                    return hasCredentials
                        ? new Socks4Client(host, port, new NetworkCredential(userName, password))
                        : new Socks4Client(host, port);
                case "socks4a":
                    return hasCredentials
                        ? new Socks4aClient(host, port, new NetworkCredential(userName, password))
                        : new Socks4aClient(host, port);
                case "socks5":
                    return hasCredentials
                        ? new Socks5Client(host, port, new NetworkCredential(userName, password))
                        : new Socks5Client(host, port);
                case "http":
                    return hasCredentials
                        ? new HttpProxyClient(host, port, new NetworkCredential(userName, password))
                        : new HttpProxyClient(host, port);
                case "https":
                    var httpsProxyClient = hasCredentials
                        ? new HttpsProxyClient(host, port, new NetworkCredential(userName, password))
                        : new HttpsProxyClient(host, port);
                    httpsProxyClient.CheckCertificateRevocation = checkCertificateRevocation;
                    if (!sslProtocols.IsNullOrEmpty()
                        && Enum.TryParse<SslProtocols>(sslProtocols, out var proxySslProtocols))
                    {
                        httpsProxyClient.SslProtocols = proxySslProtocols;
                    }
                    else if (!sslProtocols.IsNullOrEmpty())
                    {
                        new SysLogModel(
                            context: context,
                            method: nameof(CreateProxyClient),
                            message: $"Invalid SmtpProxySslProtocols: {sslProtocols}",
                            sysLogType: SysLogModel.SysLogTypes.Warning);
                    }
                    if (clientCertificate != null
                        && !clientCertificate.FindValue.IsNullOrEmpty())
                    {
                        httpsProxyClient.ClientCertificates = GetClientCertificates(clientCertificate);
                    }
                    if (serverCertificateValidationCallback)
                    {
                        httpsProxyClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    }
                    return httpsProxyClient;
                default:
                    var message = $"Unsupported proxy type: {proxyType}. Supported types: Socks4, Socks4a, Socks5, Http, Https";
                    new SysLogModel(
                        context: context,
                        method: nameof(CreateProxyClient),
                        message: message,
                        sysLogType: SysLogModel.SysLogTypes.Exception);
                    throw new ArgumentException(message);
            }
        }

        private static X509CertificateCollection GetClientCertificates(SigninCertificate certificate)
        {
            var certStore = new X509Store(
                certificate.StoreName.ToEnum(StoreName.My),
                certificate.StoreLocation.ToEnum(StoreLocation.CurrentUser));
            certStore.Open(OpenFlags.OpenExistingOnly);
            try
            {
                var certs = certStore.Certificates.Find(
                    certificate.X509FindType.ToEnum(X509FindType.FindByThumbprint),
                    certificate.FindValue,
                    false);
                if (certs.Count == 0)
                {
                    throw new InvalidOperationException(
                        $"Certificate not found with FindValue: {certificate.FindValue}");
                }
                return new X509CertificateCollection { certs[0] };
            }
            finally
            {
                certStore.Close();
            }
        }
    }
}
