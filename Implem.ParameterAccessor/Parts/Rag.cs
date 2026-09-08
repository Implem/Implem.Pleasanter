using System.ComponentModel;

namespace Implem.ParameterAccessor.Parts
{
    public class Rag
    {
        public bool Enabled { get; set; }

        public bool DeleteEnabled { get; set; }

        public string[] FormatTemplate { get; set; }

        public string OutputFilePath { get; set; }

        [DefaultValue(true)]
        public bool LogSucceeded { get; set; } = true;

        [DefaultValue(2)]
        public int SyncRetryCount { get; set; } = 2;

        [DefaultValue(10)]
        public int IndexCheckMaxAttempts { get; set; } = 10;

        [DefaultValue(2)]
        public int DeleteRetryCount { get; set; } = 2;

        [DefaultValue(60)]
        public int RetryAfterMaxSeconds { get; set; } = 60;
    }
}
