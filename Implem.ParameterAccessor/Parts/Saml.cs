using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class Saml
    {
        public Dictionary<string,string> Attributes;
        public int SamlTenantId;
        public SPOptions SPOptions;
        public List<IdentityProvider> IdentityProviders; 
    }
}
