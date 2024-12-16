using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Mails;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.IO;
using System.Linq;
using System.Text;
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
                        // 下記のエラーが発生する環境では、Mail.jsonのServerCertificateValidationCallbackをtrueに設定する
                        // The server's SSL certificate could not be validated for the following reasons
                        smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    }
                    var options = Enum.TryParse<MailKit.Security.SecureSocketOptions>(Parameters.Mail.SecureSocketOptions, out var op)
                        ? op
                        : (Parameters.Mail.SmtpEnableSsl
                            ? MailKit.Security.SecureSocketOptions.StartTls
                            : MailKit.Security.SecureSocketOptions.None);
                    await smtpClient.ConnectAsync(Host, Port, options);
                    if (!Parameters.Mail.SmtpUserName.IsNullOrEmpty()
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
                    errStackTrace: $"Supported Encodings are { Encoding.GetEncodings().Select(o => o.Name).Join(",") }.",
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
            if(Enum.TryParse(contentEncoding.ToString(), out ContentEncoding type)
                && Enum.IsDefined(typeof(ContentEncoding), type))
            {
                return type;
            }
            return ContentEncoding.Default;
        }
    }
}