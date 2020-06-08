using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Mails;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public class SendGridMail
    {
        public string Host;
        public EmailAddress From;
        public EmailAddress To;
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
            From = new EmailAddress(from.Address);
            To = new EmailAddress(Strings.CoalesceEmpty(
                to, Parameters.Mail.FixedFrom, Parameters.Mail.SupportFrom));
            Cc = cc;
            Bcc = bcc;
            Subject = subject;
            Body = body;
        }

        public async Task SendAsync(Context context)
        {
            try
            {
                var msg = MailHelper.CreateSingleEmail(From, To, Subject, Body, Body);
                Addresses.Get(
                    context: context,
                    addresses: Cc)
                        .ForEach(cc => msg.AddCc(cc));
                Addresses.Get(
                    context: context,
                    addresses: Bcc)
                        .ForEach(bcc => msg.AddBcc(bcc));
                var client = new SendGridClient(Parameters.Mail.SmtpPassword);
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception e)
            {
                new SysLogModel(Context, e);
            }
        }

        private IDictionary<string, IDictionary<string, string>> CreateAddressInfo(string address)
        {
            var mailAddress = new MailAddress(address);
            return new Dictionary<string, IDictionary<string, string>>
            {
                [mailAddress.Address] = new Dictionary<string, string>
                {
                    ["DisplayName"] = mailAddress.DisplayName.IsNullOrEmpty()
                        ? mailAddress.Address
                        : mailAddress.DisplayName
                }
            };
        }

        private MailAddress CreateMailAddress(string address)
        {
            var mailAddress = new MailAddress(address);
            return new MailAddress(
                mailAddress.Address,
                mailAddress.DisplayName.IsNullOrEmpty()
                    ? mailAddress.Address
                    : mailAddress.DisplayName);
        }
    }
}