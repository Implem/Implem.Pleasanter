using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Mails;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
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

        public void SendAsync(Context context, Attachments attachments = null)
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
                    attachments
                        ?.Where(attachment => attachment?.Base64?.IsNullOrEmpty() == false)
                        .ForEach(attachment =>
                        {
                            var bytes = Convert.FromBase64String(attachment.Base64);
                            using (var memoryStream = new MemoryStream(bytes))
                            {
                                var utfstr = Encoding.GetEncoding("iso-2022-jp").GetBytes(Strings.CoalesceEmpty(attachment.Name, "NoName"));
                                var base64str = Convert.ToBase64String(utfstr);
                                var sendName = "=?iso-2022-jp?B?" + base64str + "?=";
                                sendGridMessage.AddAttachment(
                                    stream: memoryStream,
                                    name: sendName);
                            }
                        });
                    if (Parameters.Mail.SmtpUserName.ToLower() == "apikey")
                    {
                        new SendGrid.Web(apiKey: Parameters.Mail.SmtpPassword)
                            .DeliverAsync(sendGridMessage);
                    }
                    else
                    {
                        new SendGrid.Web(new System.Net.NetworkCredential(
                            Parameters.Mail.SmtpUserName,
                            Parameters.Mail.SmtpPassword))
                                .DeliverAsync(sendGridMessage);
                    }
                }
                catch (Exception e)
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