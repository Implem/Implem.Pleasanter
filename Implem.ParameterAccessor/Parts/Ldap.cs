using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class Ldap
    {
        public string LdapSearchRoot;
        public string LdapLoginPattern;
        public string LdapSearchProperty;
        public string LdapSearchPattern;
        public string LdapAuthenticationType;
        public string NetBiosDomainName;
        public int LdapTenantId;
        public string LdapDeptCode;
        public string LdapDeptCodePattern;
        public string LdapDeptName;
        public string LdapDeptNamePattern;
        public string LdapUserCode;
        public string LdapUserCodePattern;
        public string LdapFirstName;
        public string LdapFirstNamePattern;
        public string LdapLastName;
        public string LdapLastNamePattern;
        public string LdapMailAddress;
        public string LdapMailAddressPattern;
        public List<LdapExtendedAttribute> LdapExtendedAttributes;
        public int LdapSyncPageSize;
        public List<string> LdapSyncPatterns;
        public bool LdapExcludeAccountDisabled;
        public bool AutoDisable;
        public bool AutoEnable;
        public string LdapSyncUser;
        public string LdapSyncPassword;
    }
}
