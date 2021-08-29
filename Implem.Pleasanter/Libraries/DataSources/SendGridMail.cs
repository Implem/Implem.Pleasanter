using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Mails;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public class SendGridMail
    {
        public string Host;
        public EmailAddress From;
        public string To;
        public string Cc;
        public string Bcc;
        public string Subject;
        public string Body;
        public Context Context;

        public SendGridMail(
            Context context,
            string host,
            MailAddress from,
            string to,
            string cc,
            string bcc,
            string subject,
            string body)
        {
            Context = context;
            Host = host;
            From = new EmailAddress(from.Address, from.DisplayName);
            To = Strings.CoalesceEmpty(to, Parameters.Mail.FixedFrom, Parameters.Mail.SupportFrom);
            Cc = cc;
            Bcc = bcc;
            Subject = subject;
            Body = body;
        }

        public async Task SendAsync(Context context, Attachments attachments = null)
        {
            try
            {
                var existAddresses = new List<string>();
                var client = new SendGridClient(Parameters.Mail.SmtpPassword);
                var msg = new SendGridMessage()
                {
                    From = From,
                    Subject = Subject,
                    PlainTextContent = Body
                };
                Addresses.Get(
                    context: context,
                    addresses: To)
                        .ForEach(to =>
                        {
                            var mailAddress = new MailAddress(to);
                            existAddresses.Add(mailAddress.Address);
                            msg.AddTo(mailAddress.Address, mailAddress.DisplayName);
                        });
                Addresses.Get(
                    context: context,
                    addresses: Cc)
                        .ForEach(cc =>
                        {
                            var mailAddress = new MailAddress(cc);
                            if (!existAddresses.Contains(mailAddress.Address))
                            {
                                existAddresses.Add(mailAddress.Address);
                                msg.AddCc(mailAddress.Address, mailAddress.DisplayName);
                            }
                        });
                Addresses.Get(
                    context: context,
                    addresses: Bcc)
                        .ForEach(bcc =>
                        {
                            var mailAddress = new MailAddress(bcc);
                            if (!existAddresses.Contains(mailAddress.Address))
                            {
                                existAddresses.Add(mailAddress.Address);
                                msg.AddBcc(mailAddress.Address, mailAddress.DisplayName);
                            }
                        });
                attachments
                    ?.Where(attachment => attachment?.Base64?.IsNullOrEmpty() == false)
                    .ForEach(attachment =>
                    {
                        msg.AddAttachment(
                            filename: Strings.CoalesceEmpty(attachment.Name, "NoName"),
                            base64Content: attachment.Base64,
                            type: attachment.ContentType);
                    });
                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                new SysLogModel(Context, e);
            }
        }
    }
}