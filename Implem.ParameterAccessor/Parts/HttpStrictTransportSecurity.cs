using System;
using System.Collections.Generic;

namespace Implem.ParameterAccessor.Parts
{
    public class HttpStrictTransportSecurity
    {
        public bool Enabled;
        public bool Preload;
        public bool IncludeSubDomains;
        public TimeSpan MaxAge;
        public List<string> ExcludeHosts;
    }
}
