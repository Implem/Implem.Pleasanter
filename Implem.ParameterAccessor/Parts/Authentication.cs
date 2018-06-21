using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class Authentication
    {
        public string Provider;
        public string ServiceId;
        public string ExtensionUrl;
        public int PasswordExpirationPeriod;
        public bool RejectUnregisteredUser;
        public List<Ldap> LdapParameters;
    }
}