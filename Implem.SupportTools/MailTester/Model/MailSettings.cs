using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implem.SupportTools.MailTester.Model
{

    public class MailSettings
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPassword { get; set; }
        public bool SmtpEnableSsl { get; set; }
        public string FixedFrom { get; set; }
        public string AllowedFrom { get; set; }
        public string SupportFrom { get; set; }
        public string InternalDomains { get; set; }
        public string AddressValidation { get; set; }
    }
}
