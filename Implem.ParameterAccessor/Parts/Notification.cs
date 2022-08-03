using System.Collections.Generic;
using static Implem.ParameterAccessor.Parts.Types;
namespace Implem.ParameterAccessor.Parts
{
    public class Notification
    {
        public string SecurityProtocolType;
        public bool Mail;
        public bool Slack;
        public bool ChatWork;
        public bool Line;
        public bool Teams;
        public bool RocketChat;
        public bool InCircle;
        public bool HttpClient;
        public OptionTypes CopyWithNotifications;
        public List<string> ListOrder;
        public List<string> HttpClientEncodings;
    }
}
