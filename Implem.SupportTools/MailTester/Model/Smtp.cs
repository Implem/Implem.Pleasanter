using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Implem.SupportTools.MailTester.Model
{
    public class Smtp
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
        public string FixedFrom { get; set; }
        public string AllowedFrom { get; set; }
        public MailAddress From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public Smtp(
            string host,
            int port,
            string userName,
            string password,
            bool enableSsl,
            string fixedFrom,
            string allowedFrom,
            MailAddress from,
            string to,
            string cc,
            string bcc,
            string subject,
            string body)
        {
            Host = host;
            Port = port;
            UserName = userName;
            Password = password;
            FixedFrom = fixedFrom;
            AllowedFrom = allowedFrom;
            EnableSsl = enableSsl;
            From = from;
            To = to;
            Cc = cc;
            Bcc = bcc;
            Subject = subject;
            Body = body;
        }

        public void Send()
        {
            var task = Task.Run(() =>
            {
                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = Addresses.From(From, FixedFrom, AllowedFrom);
                    foreach (var to in Addresses.GetEnumerable(To)) { mailMessage.To.Add(to); };
                    foreach (var cc in Addresses.GetEnumerable(Cc)) { mailMessage.CC.Add(cc); };
                    foreach (var bcc in Addresses.GetEnumerable(Bcc)) { mailMessage.Bcc.Add(bcc); };

                    mailMessage.Subject = Subject;
                    mailMessage.Body = Body;
                    using (var smtpClient = new SmtpClient())
                    {
                        smtpClient.Host = Host;
                        smtpClient.Port = Port;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        if (UserName != null && Password != null)
                        {
                            smtpClient.Credentials = new System.Net.NetworkCredential(
                                UserName, Password);
                        }
                        smtpClient.EnableSsl = EnableSsl;
                        smtpClient.Send(mailMessage);
                        smtpClient.Dispose();
                    }
                }
            });

            try
            {
                task.Wait();
            }
            catch (AggregateException e)
            {
                e.Handle(ex => throw ex);
            }
        }
    }
}