using System.Collections.Generic;
using System.ComponentModel;
using static Implem.ParameterAccessor.Parts.Types;
namespace Implem.ParameterAccessor.Parts
{
    public enum MailProvider
    {
        Smtp,
        SendGrid,
        AwsSes
    }

    public class Mail
    {
        public MailProvider Provider { get; set; } = MailProvider.Smtp;
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPassword { get; set; }
        public bool SmtpEnableSsl { get; set; }
        public bool ServerCertificateValidationCallback { get; set; }
        public string SecureSocketOptions { get; set; }
        public bool UseOAuth { get; set; } = false;
        public string OAuthClientId { get; set; }
        public string OAuthClientSecret { get; set; }
        public string OAuthScope { get; set; }
        public string OAuthGrantType { get; set; }
        public string OAuthTokenEndpoint { get; set; }
        [DefaultValue(3600)]
        public int OAuthDefaultExpiresIn { get; set; } = 3600;
        [DefaultValue(300)]
        public int OAuthTokenRefreshBufferTime { get; set; } = 300;
        public SendGridParameter SendGrid { get; set; } = new SendGridParameter();
        public AwsSesParameter AwsSes { get; set; } = new AwsSesParameter();
        public string Encoding { get; set; }
        public ContentEncodings? ContentEncoding { get; set; }
        public string FixedFrom { get; set; }
        public List<string> AllowedFrom { get; set; }
        public string SupportFrom { get; set; }
        public string InternalDomains { get; set; }

        public class SendGridParameter
        {
            public string ApiKey { get; set; }
        }

        public class AwsSesParameter
        {
            public string AccessKeyId { get; set; }
            public string SecretAccessKey { get; set; }
            public string SessionToken { get; set; }
            public string Region { get; set; }
            public string ConfigurationSetName { get; set; }
        }
    }
}
