using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class Authentication
    {
        public string Provider;
        public int PasswordExpirationPeriod;
        public string LdapSearchRoot;
        public string LdapSearchProperty;
        public int LdapTenantId;
        public string LdapDeptCode;
        public string LdapDeptName;
        public string LdapUserCode;
        public string LdapFirstName;
        public string LdapLastName;
        public string LdapMailAddress;
        public string ServiceId;
        public string ExtensionUrl;
        public List<string> LdapSyncPatterns;
        public string LdapSyncUser;
        public string LdapSyncPassword;
    }
}
