using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Mails;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public class Smtp
    {
        public string Host;
        public int Port;
        public MailAddress From;
        public string To;
        public string Cc;
        public string Bcc;
        public string Subject;
        public string Body;

        public Smtp(
            string host,
            int port,
            MailAddress from,
            string to,
            string cc,
            string bcc,
            string subject,
            string body)
        {
            Host = host;
            Port = port;
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
                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = Addresses.From(From);
                    Addresses.GetEnumerable(To).ForEach(to => mailMessage.To.Add(to));
                    Addresses.GetEnumerable(Cc).ForEach(cc => mailMessage.CC.Add(cc));
                    Addresses.GetEnumerable(Bcc).ForEach(bcc => mailMessage.Bcc.Add(bcc));
                    mailMessage.Subject = Subject;
                    mailMessage.Body = Body;
                    using (var smtpClient = new SmtpClient())
                    {
                        smtpClient.Host = Host;
                        smtpClient.Port = Port;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        if (Parameters.Mail.SmtpUserName != null &&
                            Parameters.Mail.SmtpPassword != null)
                        {
                            smtpClient.Credentials = new System.Net.NetworkCredential(
                                Parameters.Mail.SmtpUserName, Parameters.Mail.SmtpPassword);
                        }
                        smtpClient.EnableSsl = Parameters.Mail.SmtpEnableSsl;
                        smtpClient.Send(mailMessage);
                        smtpClient.Dispose();
                    }
                }
            });
        }
    }
}