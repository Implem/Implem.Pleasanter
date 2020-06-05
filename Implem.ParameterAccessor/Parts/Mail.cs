using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class Mail
    {
        public string SmtpHost;
        public int SmtpPort;
        public string SmtpUserName;
        public string SmtpPassword;
        public bool SmtpEnableSsl;
        public string FixedFrom;
        public List<string> AllowedFrom;
        public string SupportFrom;
        public string InternalDomains;
    }
}
