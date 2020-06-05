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

        public static List<Notification> Get(SiteSettings ss, Context context)
        {
            var notifications = context.Forms.Get("Notifications")
                ?.Deserialize<SettingList<Notification>>()
                    ?? ss.Notifications;
            notifications
                .Select((o, i) => new { Index = i, Notification = o })
                .ForEach(data =>
                {
                    data.Notification.Index = data.Index;
                    data.Notification.MonitorChangesColumns = data.Notification.MonitorChangesColumns
                        ?? ss.GetEditorColumnNames(
                            context: context,
                            columnOnly: true);
                });
            return notifications;
        }

        public static List<Notification> MeetConditions(
            SiteSettings ss, List<Notification> before, List<Notification> after)
        {
            var data = new List<Notification>();
            if (before != null)
            {
                data.AddRange(before);
            }
            if (after != null)
            {
                data.AddRange(after);
            }
            return data
                .Where(o => data.Where(p => p.Index == o.Index).Count() == 2
                    || (data.Where(p => p.Index == o.Index).Count() == 1
                        && o.Expression == Notification.Expressions.Or
                        && (ss.Views?.Any(p => p.Id == o.BeforeCondition) == true
                            && ss.Views?.Any(p => p.Id == o.AfterCondition) == true)))
                .GroupBy(o => o.Index)
                .Select(o => o.First())
                .ToList();
        }
    }
}