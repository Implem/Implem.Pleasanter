using Implem.Libraries.DataSources.SqlServer;

namespace Implem.DefinitionAccessor.Parameters
{
    public class Rds
    {
        public string Provider;
        public string TimeZoneInfo;
        public string SaConnectionString;
        public string OwnerConnectionString;
        public string UserConnectionString;
        public int SqlCommandTimeOut;
        public int SqlAzureRetryCount;
        public int SqlAzureRetryInterval;
    }
}
