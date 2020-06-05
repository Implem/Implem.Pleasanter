using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Mails;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public class SendGridMail
    {
        public string Host;
        public MailAddress From;
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
            From = from;
            To = Strings.CoalesceEmpty(to, Parameters.Mail.FixedFrom, Parameters.Mail.SupportFrom);
            Cc = cc;
            Bcc = bcc;
            Subject = subject;
            Body = body;
        }

        public void Send(Context context)
        {
            Task.Run(() =>
            {
                try
                {
                    var sendGridMessage = new SendGrid.SendGridMessage();
                    sendGridMessage.From = Addresses.From(From);
                    Addresses.Get(
                        context: context,
                        addresses: To)
                           .ForEach(to => sendGridMessage.AddTo(CreateAddressInfo(to)));
                    Addresses.Get(
                        context: context,
                        addresses: Cc)
                            .ForEach(cc => sendGridMessage.AddCc(CreateMailAddress(cc)));
                    Addresses.Get(
                        context: context,
                        addresses: Bcc)
                            .ForEach(bcc => sendGridMessage.AddBcc(CreateMailAddress(bcc)));
                    sendGridMessage.Subject = Subject;
                    sendGridMessage.Text = Body;
                    new SendGrid.Web(new System.Net.NetworkCredential(
                        Parameters.Mail.SmtpUserName,
                        Parameters.Mail.SmtpPassword))
                            .DeliverAsync(sendGridMessage);
                }
                catch(Exception e)
                {
                    new SysLogModel(Context, e);
                }
            });
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