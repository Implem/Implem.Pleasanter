using System;

namespace Implem.ParameterAccessor.Parts
{
    [Flags]
    public enum LicenseOptions
    {
        None = 0,
        AdvancedCache = 1 << 0,
        MultiTenants = 1 << 1,
        Queue = 1 << 2,
        RateLimit = 1 << 3,
        SamlExtendedAttributes = 1 << 4,
        Scim = 1 << 5,
        BlockSiteTaskWhileRunning = 1 << 6,

        Trial = AdvancedCache
            | Queue
            | RateLimit
            | SamlExtendedAttributes
            | Scim
            | BlockSiteTaskWhileRunning
    }
}