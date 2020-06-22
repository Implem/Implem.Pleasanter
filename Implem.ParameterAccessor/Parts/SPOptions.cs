using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Implem.ParameterAccessor.Parts
{
    public class SPOptions
    {
        public string EntityId;
        public string ReturnUrl;
        public string AuthenticateRequestSigningBehavior;
        public string OutboundSigningAlgorithm;
        public string MinIncomingSigningAlgorithm;
        public List<ServiceCertificate> ServiceCertificates;
    }

    public class ServiceCertificate
    {
        public string Use;
        public string Status;
        public string MetadataPublishOverride;
        public string StoreName;
        public string StoreLocation;
        public string X509FindType;
        public string FindValue;

        public X509Certificate2 Certificate { get; set; }
    }
}
