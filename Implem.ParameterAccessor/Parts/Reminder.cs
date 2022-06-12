using System.Collections.Generic;
using static Implem.ParameterAccessor.Parts.Types;
namespace Implem.ParameterAccessor.Parts
{
    public class Reminder
    {
        public bool Enabled;
        public bool Mail;
        public bool Slack;
        public bool ChatWork;
        public bool Line;
        public bool Teams;
        public bool RocketChat;
        public bool InCircle;
        public int Interval;
        public int Span;
        public int Limit;
        public string DefaultLine;
        public int MinRange;
        public int MaxRange;
        public int DefaultRange;
        public OptionTypes CopyWithReminders;
        public List<string> ListOrder;
    }
}