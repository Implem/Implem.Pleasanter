using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Mails;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public class AwsSesMail
    {
        public MailboxAddress From;
        public string To;
        public string Cc;
        public string Bcc;
        public string Subject;
        public string Body;
        public Context Context;

        private static readonly System.Net.Http.HttpClient HttpClient = new System.Net.Http.HttpClient();

        public AwsSesMail(
            Context context,
            MailboxAddress from,
            string to,
            string cc,
            string bcc,
            string subject,
            string body)
        {
            Context = context;
            From = Addresses.From(from);
            To = to;
            Cc = cc;
            Bcc = bcc;
            Subject = subject;
            Body = body;
        }

        public async Task SendAsync(Context context, Attachments attachments = null)
        {
            try
            {
                await SendEmailAsync(context, attachments);
            }
            catch (Exception e)
            {
                new SysLogModel(Context, e);
            }
        }

        private async Task SendEmailAsync(
            Context context,
            Attachments attachments)
        {
            ValidateConfiguration();
            var message = new MimeMessage();
            var enc = MailEncodings.GetEncodingOrDefault(context: context,
                encoding: Parameters.Mail.Encoding);
            message.From.Add(From.SetEncoding(enc));
            Addresses.Get(context: context, addresses: To)
                .ForEach(to => message.To.Add(MailboxAddress.Parse(to).SetEncoding(enc)));
            Addresses.Get(context: context, addresses: Cc)
                .ForEach(cc => message.Cc.Add(MailboxAddress.Parse(cc).SetEncoding(enc)));
            Addresses.Get(context: context, addresses: Bcc)
                .ForEach(bcc => message.Bcc.Add(MailboxAddress.Parse(bcc).SetEncoding(enc)));
            message.Headers.Replace(HeaderId.Subject, enc, Subject);

            var textPart = new TextPart(MimeKit.Text.TextFormat.Plain);
            textPart.SetText(enc, Body);
            textPart.ContentTransferEncoding = MailEncodings.GetContentEncodingForTransfer(
                encoding: enc,
                contentEncoding: Parameters.Mail.ContentEncoding);

            var validAttachments = attachments?
                .Where(a => a?.Base64?.IsNullOrEmpty() == false)
                .ToList();
            var streams = new List<MemoryStream>();
            try
            {
                if (validAttachments?.Any() == true)
                {
                    var multipart = new Multipart("mixed") { textPart };
                    validAttachments.ForEach(attachment =>
                    {
                        var fileName = attachment.Name ?? attachment.FileName ?? string.Empty;
                        var mimeType = attachment.ContentType.IsNullOrEmpty()
                            ? MimeTypes.GetMimeType(fileName)
                            : attachment.ContentType;
                        var stream = new MemoryStream(Convert.FromBase64String(attachment.Base64));
                        streams.Add(stream);
                        var mimePart = new MimePart(mimeType)
                        {
                            FileName = Strings.CoalesceEmpty(fileName, "NoName"),
                            Content = new MimeContent(stream),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64
                        };
                        multipart.Add(mimePart);
                    });
                    message.Body = multipart;
                }
                else
                {
                    message.Body = textPart;
                }

                using var mimeStream = new MemoryStream();
                await message.WriteToAsync(mimeStream).ConfigureAwait(false);
                var rawMessageBytes = mimeStream.ToArray();

                var requestBody = new Dictionary<string, object>
                {
                    ["Content"] = new Dictionary<string, object>
                    {
                        ["Raw"] = new Dictionary<string, string>
                        {
                            ["Data"] = Convert.ToBase64String(rawMessageBytes)
                        }
                    }
                };

                if (!Parameters.Mail.AwsSes.ConfigurationSetName.IsNullOrEmpty())
                {
                    requestBody["ConfigurationSetName"] = Parameters.Mail.AwsSes.ConfigurationSetName;
                }

                var jsonBody = JsonSerializer.Serialize(requestBody);
                var region = Parameters.Mail.AwsSes.Region;
                var host = $"email.{region}.amazonaws.com";
                var endpoint = $"https://{host}/v2/email/outbound-emails";
                var now = DateTime.UtcNow;

                using var request = CreateSignedRequest(
                    method: HttpMethod.Post,
                    uri: new Uri(endpoint),
                    host: host,
                    region: region,
                    service: "ses",
                    jsonBody: jsonBody,
                    now: now,
                    accessKeyId: Parameters.Mail.AwsSes.AccessKeyId,
                    secretAccessKey: Parameters.Mail.AwsSes.SecretAccessKey,
                    sessionToken: Parameters.Mail.AwsSes.SessionToken);
                using var response = await HttpClient.SendAsync(request).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var errorCode = ExtractErrorCode(errorContent);
                    throw new HttpRequestException(
                        $"SES API request failed with status {(int)response.StatusCode} ({response.StatusCode})"
                        + (errorCode != null ? $", Code={errorCode}" : string.Empty));
                }
            }
            finally
            {
                streams.ForEach(s => s.Dispose());
            }
        }

        private static HttpRequestMessage CreateSignedRequest(
            HttpMethod method,
            Uri uri,
            string host,
            string region,
            string service,
            string jsonBody,
            DateTime now,
            string accessKeyId,
            string secretAccessKey,
            string sessionToken = null)
        {
            var dateStamp = now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            var amzDate = now.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);
            var bodyBytes = Encoding.UTF8.GetBytes(jsonBody);
            var payloadHash = HexEncode(SHA256.HashData(bodyBytes));
            var hasSessionToken = !sessionToken.IsNullOrEmpty();

            var canonicalUri = uri.AbsolutePath;
            var canonicalQuerystring = string.Empty;
            var signedHeaders = hasSessionToken
                ? "content-type;host;x-amz-content-sha256;x-amz-date;x-amz-security-token"
                : "content-type;host;x-amz-content-sha256;x-amz-date";
            var canonicalHeaders =
                $"content-type:application/json\n" +
                $"host:{host}\n" +
                $"x-amz-content-sha256:{payloadHash}\n" +
                $"x-amz-date:{amzDate}\n";
            if (hasSessionToken)
            {
                canonicalHeaders += $"x-amz-security-token:{sessionToken}\n";
            }
            var canonicalRequest =
                $"{method.Method}\n" +
                $"{canonicalUri}\n" +
                $"{canonicalQuerystring}\n" +
                canonicalHeaders +
                $"\n" +
                $"{signedHeaders}\n" +
                payloadHash;

            var credentialScope = $"{dateStamp}/{region}/{service}/aws4_request";
            var stringToSign =
                $"AWS4-HMAC-SHA256\n" +
                $"{amzDate}\n" +
                $"{credentialScope}\n" +
                HexEncode(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalRequest)));

            var signingKey = GetSignatureKey(secretAccessKey, dateStamp, region, service);

            var signature = HexEncode(HMACSHA256.HashData(signingKey, Encoding.UTF8.GetBytes(stringToSign)));

            var authorizationHeader =
                $"AWS4-HMAC-SHA256 Credential={accessKeyId}/{credentialScope}, " +
                $"SignedHeaders={signedHeaders}, " +
                $"Signature={signature}";

            var request = new HttpRequestMessage(method, uri)
            {
                Content = new ByteArrayContent(bodyBytes)
            };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Headers.Add("x-amz-content-sha256", payloadHash);
            request.Headers.Add("x-amz-date", amzDate);
            if (hasSessionToken)
            {
                request.Headers.Add("x-amz-security-token", sessionToken);
            }
            request.Headers.TryAddWithoutValidation("Authorization", authorizationHeader);
            return request;
        }

        private static byte[] GetSignatureKey(
            string key,
            string dateStamp,
            string region,
            string service)
        {
            var kDate = HMACSHA256.HashData(
                Encoding.UTF8.GetBytes("AWS4" + key),
                Encoding.UTF8.GetBytes(dateStamp));
            var kRegion = HMACSHA256.HashData(kDate, Encoding.UTF8.GetBytes(region));
            var kService = HMACSHA256.HashData(kRegion, Encoding.UTF8.GetBytes(service));
            return HMACSHA256.HashData(kService, Encoding.UTF8.GetBytes("aws4_request"));
        }

        private static string HexEncode(byte[] bytes)
        {
            return Convert.ToHexStringLower(bytes);
        }

        private static void ValidateConfiguration()
        {
            if (Parameters.Mail.AwsSes.Region.IsNullOrEmpty())
            {
                throw new InvalidOperationException(
                    "AWS SES configuration 'Region' is not set. Please check Mail.json > AwsSes > Region in Parameters.");
            }
            if (Parameters.Mail.AwsSes.AccessKeyId.IsNullOrEmpty())
            {
                throw new InvalidOperationException(
                    "AWS SES configuration 'AccessKeyId' is not set. Please check Mail.json > AwsSes > AccessKeyId in Parameters.");
            }
            if (Parameters.Mail.AwsSes.SecretAccessKey.IsNullOrEmpty())
            {
                throw new InvalidOperationException(
                    "AWS SES configuration 'SecretAccessKey' is not set. Please check Mail.json > AwsSes > SecretAccessKey in Parameters.");
            }
        }

        private static string ExtractErrorCode(string errorContent)
        {
            try
            {
                using var doc = JsonDocument.Parse(errorContent);
                if (doc.RootElement.TryGetProperty("Code", out var code))
                {
                    return code.GetString();
                }
            }
            catch (JsonException)
            {
            }
            return null;
        }
    }
}
