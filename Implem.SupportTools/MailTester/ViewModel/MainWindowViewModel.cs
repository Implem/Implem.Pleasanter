using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System.IO;
using System.Windows.Input;
using Implem.SupportTools.MailTester.Model;
using Implem.SupportTools.Common;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Implem.SupportTools.MailTester.ViewModel
{
    public class MailTesterWindowViewModel : BindableBase
    {
        private readonly string pleasanterSettingsPath;
        private readonly string mailTesterSettingsPath = "Settings\\MailTester.json";
        private readonly string mailjsonPath = "";
        private readonly ILogger Logger;

        private string smtpHost;
        private int smtpPort;
        private string smtpUserName;
        private string smtpPassword;
        private bool smtpEnableSsl;
        private string fixedFrom;
        private string allowedFrom;
        private string supportFrom;
        private string internalDomains;
        private string to;
        private string cc;
        private string bcc;
        private string subject;
        private string body;

        public string SmtpHost { get => smtpHost; set { SetProperty(ref smtpHost, value); } }
        public int SmtpPort { get => smtpPort; set { SetProperty(ref smtpPort, value); } }
        public string SmtpUserName { get => smtpUserName; set { SetProperty(ref smtpUserName, value); } }
        public string SmtpPassword { get => smtpPassword; set { SetProperty(ref smtpPassword, value); } }
        public bool SmtpEnableSsl { get => smtpEnableSsl; set { SetProperty(ref smtpEnableSsl, value); } }
        public string FixedFrom { get => fixedFrom; set { SetProperty(ref fixedFrom, value); } }
        public string AllowedFrom { get => allowedFrom; set { SetProperty(ref allowedFrom, value); } }
        public string SupportFrom { get => supportFrom; set { SetProperty(ref supportFrom, value); } }
        public string InternalDomains { get => internalDomains; set { SetProperty(ref internalDomains, value); } }

        public string To { get => to; set { SetProperty(ref to, value); } }
        public string Cc { get => cc; set { SetProperty(ref cc, value); } }
        public string Bcc { get => bcc; set { SetProperty(ref bcc, value); } }
        public string Subject { get => subject; set { SetProperty(ref subject, value); } }
        public string Body { get => body; set { SetProperty(ref body, value); } }

        public ICommand SendMailCommand { set; get; }

        
        public MailTesterWindowViewModel(ILogger logger, string pleasanterSettingsPath)
        {
            Logger = logger;
            this.pleasanterSettingsPath = pleasanterSettingsPath;
            mailjsonPath = $@"{pleasanterSettingsPath}\Parameters\Mail.json";

            if (File.Exists(mailjsonPath))
            {
                var json = File.ReadAllText(mailjsonPath);
                var mailSettings = JsonConvert.DeserializeObject<MailSettings>(json);
                SmtpHost = mailSettings.SmtpHost;
                SmtpPort = mailSettings.SmtpPort;
                SmtpUserName = mailSettings.SmtpUserName;
                SmtpPassword = mailSettings.SmtpPassword;
                SmtpEnableSsl = mailSettings.SmtpEnableSsl;
                FixedFrom = mailSettings.FixedFrom;
                AllowedFrom = mailSettings.AllowedFrom;
                SupportFrom = mailSettings.SupportFrom;
                InternalDomains = mailSettings.InternalDomains;
            }
            
            if (File.Exists(mailTesterSettingsPath))
            {
                var json = File.ReadAllText(mailTesterSettingsPath);
                var sendData = JsonConvert.DeserializeObject<SendData>(json);
                To = sendData.To;
                Cc = sendData.Cc;
                Bcc = sendData.Bcc;
                Subject = sendData.Subject;
                Body = sendData.Body;
            }

            SendMailCommand = new DelegateCommand(() => SendMail());
        }

        public void SendMail()
        {
            var badAddressTo = Addresses.BadAddresses(To);
            if (badAddressTo.Any())
            {
                Logger.Error(nameof(MailTester), $"There is an invalid email address in the [To] field. ({ string.Join(",", badAddressTo)})");
            }
            var badAddressCc = Addresses.BadAddresses(Cc);
            if (badAddressCc.Any())
            {
                Logger.Error(nameof(MailTester), $"There is an invalid email address in the [CC] field. ({ string.Join(",", badAddressCc)})");
            }
            var badAddressBcc = Addresses.BadAddresses(Bcc);
            if (badAddressBcc.Any())
            {
                Logger.Error(nameof(MailTester), $"There is an invalid email address in the [BCC] field. ({ string.Join(",", badAddressBcc)})");
            }
            var smtp = new Smtp(
                SmtpHost,
                SmtpPort,
                SmtpUserName,
                SmtpPassword,
                SmtpEnableSsl,
                FixedFrom,
                AllowedFrom,
                new System.Net.Mail.MailAddress(SupportFrom),
                To,
                Cc,
                Bcc,
                Subject,
                Body);
            try
            {
                smtp.Send();
                Logger.Info(nameof(MailTester), "SMTP Send Completed.");
            }
            catch(Exception e)
            {
                Logger.Error(nameof(MailTester), "SMTP Send Error.", e);
            }
        }

        public void SaveSettings()
        {
            var mailjson = JsonConvert.SerializeObject(new MailSettings()
            {
                SmtpHost = SmtpHost,
                SmtpPort = SmtpPort,
                SmtpUserName = SmtpUserName,
                SmtpPassword = SmtpPassword,
                SmtpEnableSsl = SmtpEnableSsl,
                FixedFrom = FixedFrom,
                AllowedFrom = AllowedFrom,
                SupportFrom = SupportFrom,
                InternalDomains = InternalDomains
            }, Formatting.Indented);

            File.WriteAllText(mailjsonPath, mailjson);

            var sendDataJson = JsonConvert.SerializeObject(new SendData()
            {
                To = To,
                Cc = Cc,
                Bcc = Bcc,
                Subject = Subject,
                Body = Body
            });

            File.WriteAllText(mailjsonPath, sendDataJson);
            System.Diagnostics.Trace.Flush();
        }
    }
}
