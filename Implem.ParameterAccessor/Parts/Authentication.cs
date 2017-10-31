using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class Authentication
    {
        public string Provider;
        public int PasswordExpirationPeriod;
        public string ServiceId;
        public string ExtensionUrl;
        public List<Ldap> LdapParameters;
    }
}
