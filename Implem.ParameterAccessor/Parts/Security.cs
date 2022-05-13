using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class Security
    {
        public bool MimeTypeCheckOnApi;
        public List<string> PrivilegedUsers;
        public bool RevealUserDisabled;
        public int LockoutCount;
        public int PasswordExpirationPeriod;
        public bool JoeAccountCheck;
        public bool TokenCheck;
        public bool SecureCookies;
        public bool DisableMvcResponseHeader;
        public bool DisableSiteDeletingLogin;
        public List<string> AccessControlAllowOrigin;
        public int EnforcePasswordHistories;
        public List<PasswordPolicy> PasswordPolicies;
        public SecondaryAuthentication SecondaryAuthentication;
    }
}
