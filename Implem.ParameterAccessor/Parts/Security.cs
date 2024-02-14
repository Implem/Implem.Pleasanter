using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class Security
    {
        public List<string> AllowIpAddresses;
        public bool MimeTypeCheckOnApi;
        public List<string> PrivilegedUsers;
        public bool RevealUserDisabled;
        public int LockoutCount;
        public int PasswordExpirationPeriod;
        public bool JoeAccountCheck;
        public bool TokenCheck;
        public bool SecureCookies;
        public bool DisableMvcResponseHeader;
        public bool DisableDeletingSiteAuthentication;
        public List<string> AccessControlAllowOrigin;
        public int EnforcePasswordHistories;
        public bool DisableCheckPasswordPolicyIfApi;
        public List<PasswordPolicy> PasswordPolicies;
        public SecondaryAuthentication SecondaryAuthentication;
        public bool PasswordGenerator;
        public AspNetCoreDataProtection AspNetCoreDataProtection;
        public HttpStrictTransportSecurity HttpStrictTransportSecurity;
        public SecureCacheControl SecureCacheControl;
    }
}
