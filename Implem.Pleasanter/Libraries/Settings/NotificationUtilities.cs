using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
using System.Data;
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

        public static void CheckConditions(
            this SettingList<Notification> notifications,
            List<View> views,
            bool before,
            DataSet dataSet)
        {
            notifications
                .Select((o, i) => new
                {
                    Notification = o,
                    Exists = dataSet.Tables[i].Rows.Count == 1
                })
                .ForEach(o =>
                {
                    if (before)
                    {
                        o.Notification.Enabled = o.Exists;
                    }
                    else if (views?.Get(o.Notification.AfterCondition) != null)
                    {
                        if (views?.Get(o.Notification.BeforeCondition) == null)
                        {
                            o.Notification.Enabled = o.Exists;
                        }
                        else if (o.Notification.Expression == Notification.Expressions.And)
                        {
                            o.Notification.Enabled &= o.Exists;
                        }
                        else
                        {
                            o.Notification.Enabled |= o.Exists;
                        }
                    }
                });
        }
    }
}