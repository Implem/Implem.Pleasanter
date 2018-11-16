using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Mails;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System;
using System.Linq;
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
                    Addresses.GetEnumerable(
                        context: context,
                        addresses: To)
                            .ForEach(to => sendGridMessage.AddTo(to));
                    Addresses.GetEnumerable(
                        context: context,
                        addresses: Cc)
                            .ForEach(cc => sendGridMessage.AddCc(cc));
                    Addresses.GetEnumerable(
                        context: context,
                        addresses: Bcc)
                            .ForEach(bcc => sendGridMessage.AddBcc(bcc));
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
    }
}