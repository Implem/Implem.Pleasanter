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

        Trial = AdvancedCache | Queue | RateLimit
    }
}