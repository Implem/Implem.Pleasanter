using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class Ldap
    {
        public string LdapSearchRoot;
        public string LdapSearchProperty;
        public int LdapTenantId;
        public string LdapDeptCode;
        public string LdapDeptName;
        public string LdapUserCode;
        public string LdapFirstName;
        public string LdapLastName;
        public string LdapMailAddress;
        public List<string> LdapSyncPatterns;
        public bool LdapExcludeAccountDisabled;
        public string LdapSyncUser;
        public string LdapSyncPassword;
    }
}
