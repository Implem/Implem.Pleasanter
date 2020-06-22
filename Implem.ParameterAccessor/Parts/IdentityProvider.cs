using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class IdentityProvider
    {
        public string EntityId;
        public string SignOnUrl;
        public string LogoutUrl;
        public bool AllowUnsolicitedAuthnResponse;
        public string Binding;
        public bool WantAuthnRequestsSigned;
        public bool LoadMetadata;
        public string MetadataLocation;
        public bool DisableOutboundLogoutRequests;
        public SigninCertificate SigningCertificate;
    }

    public class SigninCertificate { 
        public string StoreName;
        public string StoreLocation;
        public string FindValue;
        public string X509FindType;
    }
}
