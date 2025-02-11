using System.Collections.Generic;
using System.ComponentModel;
namespace Implem.ParameterAccessor.Parts
{
    public class SysLog
    {
        public int RetentionPeriod { get; set; }
        public List<string> NotLoggingIp { get; set; }
        public bool LoginSuccess { get; set; }
        public bool LoginFailure { get; set; }
        public bool SignOut { get; set; }
        public bool ClientId { get; set; }
        public int ExportLimit { get; set; }
        [DefaultValue(true)]
        public bool EnableLoggingToDatabase { get; set; } = true;
        public bool EnableLoggingToFile { get; set; }
        [DefaultValue(true)]
        public bool LogLevelDetails { get; set; } = true;
    }
}
