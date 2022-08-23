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
                message.From.Add(Addresses.From(From));
                Addresses.Get(
                    context: context,
                    addresses: To)
                    .ForEach(to => message.To.Add(MailboxAddress.Parse(to)));
                Addresses.Get(
                    context: context,
                    addresses: Cc)
                    .ForEach(cc => message.Cc.Add(MailboxAddress.Parse(cc)));
                Addresses.Get(
                    context: context,
                    addresses: Bcc)
                        .ForEach(bcc => message.Bcc.Add(MailboxAddress.Parse(bcc)));
                message.Subject = Subject;
                var textPart = new TextPart(MimeKit.Text.TextFormat.Plain)
                {
                    Text = Body
                };
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
                    var options = Parameters.Mail.SmtpEnableSsl
                        ? MailKit.Security.SecureSocketOptions.StartTls
                        : MailKit.Security.SecureSocketOptions.None;
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
    }
}