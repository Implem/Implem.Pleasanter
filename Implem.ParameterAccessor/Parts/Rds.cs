using System.Runtime.Serialization;
namespace Implem.ParameterAccessor.Parts
{
    public class Rds
    {
        public string Dbms;
        public string Provider;
        public string TimeZoneInfo;
        public string SaConnectionString;
        public string OwnerConnectionString;
        public string UserConnectionString;
        public int SqlCommandTimeOut;
        public int MinimumTime;
        public int DeadlockRetryCount;
        public int DeadlockRetryInterval;

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            Dbms = string.IsNullOrWhiteSpace(Dbms) ? "SQLServer" : Dbms;
        }
    }
}
