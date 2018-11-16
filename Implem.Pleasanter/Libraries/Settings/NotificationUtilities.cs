using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class NotificationUtilities
    {
        public static Dictionary<string, string> Types(Context context)
        {
            var notificationType = new Dictionary<string, string>();
            if (Parameters.Notification.Mail)
            {
                notificationType.Add(
                    Notification.Types.Mail.ToInt().ToString(),
                    Displays.Mail(context: context));
            }
            if (Parameters.Notification.Slack)
            {
                notificationType.Add(
                    Notification.Types.Slack.ToInt().ToString(),
                    Displays.Slack(context: context));
            }
            if (Parameters.Notification.ChatWork)
            {
                notificationType.Add(
                    Notification.Types.ChatWork.ToInt().ToString(),
                    Displays.ChatWork(context: context));
            }
            if (Parameters.Notification.Line)
            {
                notificationType.Add(
                    Notification.Types.Line.ToInt().ToString(),
                    Displays.Line(context: context));
                notificationType.Add(
                    Notification.Types.LineGroup.ToInt().ToString(),
                    Displays.LineGroup(context: context));
            }
            if (Parameters.Notification.Teams)
            {
                notificationType.Add(
                    Notification.Types.Teams.ToInt().ToString(),
                    Displays.Teams(context: context));
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
                Notification.Types.ChatWork,
                Notification.Types.Line,
                Notification.Types.LineGroup
            };
        }

        public static SettingList<Notification> GetNotifications(this SiteSettings ss, Context context)
        {
            var notifications = context.Forms.Get("Notifications")?
                .Deserialize<SettingList<Notification>>()?
                .Where(o => o.Enabled);
            if (notifications != null)
            {
                notifications
                    .Where(o => o.MonitorChangesColumns?.Any() != true)
                    .ForEach(notification =>
                        notification.MonitorChangesColumns = ss.EditorColumns);
                return SettingList(notifications);
            }
            else
            {
                return SettingList(ss.Notifications.Where(o => o.Enabled));
            }
        }

        private static SettingList<Notification> SettingList(IEnumerable<Notification> notifications)
        {
            var list = new SettingList<Notification>();
            notifications.ForEach(notification => list.Add(notification));
            return list;
        }

        public static void CheckConditions(
            this List<Notification> notifications,
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