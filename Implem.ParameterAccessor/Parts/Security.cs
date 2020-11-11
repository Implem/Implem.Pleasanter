using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class Security
    {
        public bool MimeTypeCheckOnApi;
        public int RequestLimit;
        public List<string> PrivilegedUsers;
        public bool RevealUserDisabled;
        public int LockoutCount;
        public int PasswordExpirationPeriod;
        public bool JoeAccountCheck;
        public List<PasswordPolicy> PasswordPolicies;
        public SecondaryAuthentication SecondaryAuthentication;
    }
}
