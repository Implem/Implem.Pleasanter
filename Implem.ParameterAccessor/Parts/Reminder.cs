using static Implem.ParameterAccessor.Parts.Types;
namespace Implem.ParameterAccessor.Parts
{
    public class Reminder
    {
        public bool Enabled;
        public int Interval;
        public int Span;
        public int Limit;
        public string DefaultLine;
        public int MinRange;
        public int MaxRange;
        public int DefaultRange;
        public OptionTypes CopyWithReminders;
        public int RemindIntervalSeconds;
        public bool UseGenericHost;
        public int MaxExceptionCount;        
    }
}