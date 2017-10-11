using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class NotificationUtilities
    {
        public static Dictionary<string, string> Types()
        {
            var notificationType = new Dictionary<string, string>();
            if (Parameters.Notification.Mail)
            {
                notificationType.Add(
                    Notification.Types.Mail.ToInt().ToString(),
                    Displays.Mail());
            }
            if (Parameters.Notification.Slack)
            {
                notificationType.Add(
                    Notification.Types.Slack.ToInt().ToString(),
                    Displays.Slack());
            }
            if (Parameters.Notification.ChatWork)
            {
                notificationType.Add(
                    Notification.Types.ChatWork.ToInt().ToString(),
                    Displays.ChatWork());
            }
            return notificationType;
        }

        public static bool RequireToken(Notification notification)
        {
            return TokenList().Contains(notification.Type);
        }

        public static string Tokens()
        {
            return TokenList().Select(o => o.ToInt().ToString()).Join();
        }

        private static List<Notification.Types> TokenList()
        {
            return new List<Notification.Types>
            {
                Notification.Types.ChatWork
            };
        }
    }
}