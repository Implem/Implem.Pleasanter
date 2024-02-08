using System.Collections.Generic;
using static Implem.ParameterAccessor.Parts.Types;
namespace Implem.ParameterAccessor.Parts
{
    public class Mail
    {
        public string SmtpHost;
        public int SmtpPort;
        public string SmtpUserName;
        public string SmtpPassword;
        public bool SmtpEnableSsl;
        public bool ServerCertificateValidationCallback;
        public string SecureSocketOptions;
        public string Encoding;
        public ContentEncodings? ContentEncoding;
        public string FixedFrom;
        public List<string> AllowedFrom;
        public string SupportFrom;
        public string InternalDomains;
    }
}
