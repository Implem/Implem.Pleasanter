﻿using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class Authentication
    {
        public string Provider;
        public string DsProvider;
        public string ServiceId;
        public string ExtensionUrl;
        public bool RejectUnregisteredUser;
        public List<Ldap> LdapParameters;
        public Saml SamlParameters;
    }
}