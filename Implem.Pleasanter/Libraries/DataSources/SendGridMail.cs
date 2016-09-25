using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Mails;
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

        public SendGridMail(
            string host,
            MailAddress from,
            string to,
            string cc,
            string bcc,
            string subject,
            string body)
        {
            Host = host;
            From = from;
            To = to;
            Cc = cc;
            Bcc = bcc;
            Subject = subject;
            Body = body;
        }

        public void Send()
        {
            Task.Run(() =>
            {
                var sendGridMessage = new SendGrid.SendGridMessage();
                sendGridMessage.From = From;
                Addresses.GetEnumerable(To).ForEach(to => sendGridMessage.AddTo(to));
                Addresses.GetEnumerable(Cc).ForEach(cc => sendGridMessage.AddCc(cc));
                Addresses.GetEnumerable(Bcc).ForEach(bcc => sendGridMessage.AddBcc(bcc));
                sendGridMessage.Subject = Subject;
                sendGridMessage.Text = Body;
                new SendGrid.Web(new System.Net.NetworkCredential(
                    Parameters.Mail.SendGridSmtpUser,
                    Parameters.Mail.SendGridSmtpPassword))
                        .DeliverAsync(sendGridMessage);
            });
        }
    }
}