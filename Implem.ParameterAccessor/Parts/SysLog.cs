using System.Collections.Generic;
namespace Implem.ParameterAccessor.Parts
{
    public class SysLog
    {
        public int RetentionPeriod;
        public List<string> NotLoggingIp;
        public bool LoginSuccess;
        public bool LoginFailure;
        public bool SignOut;
        public bool ClientId;
        public int ExportLimit;
        public bool EnableLoggingToDatabase = true;
        public bool EnableLoggingToFile; //= false
    }
}
