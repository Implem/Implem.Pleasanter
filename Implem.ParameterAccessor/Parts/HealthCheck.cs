﻿namespace Implem.ParameterAccessor.Parts
{
    public class HealthCheck
    {
        public bool Enabled;
        public bool EnableDatabaseCheck;
        public string HealthQuery;
        public string[] RequireHosts;
        public bool EnableDetailedResponse;
    }
}
