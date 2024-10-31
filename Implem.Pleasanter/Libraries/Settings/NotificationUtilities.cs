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
        public static Dictionary<string, string> Types(Context context, bool isProcessNotification = false)
        {
            var notificationTypes = new Dictionary<string, string>();
            if (Parameters.Notification.Mail)
            {
                notificationTypes.Add(
                    Notification.Types.Mail.ToInt().ToString(),
                    Displays.Mail(context: context));
            }
            if (Parameters.Notification.Slack)
            {
                notificationTypes.Add(
                    Notification.Types.Slack.ToInt().ToString(),
                    Displays.Slack(context: context));
            }
            if (Parameters.Notification.ChatWork)
            {
                notificationTypes.Add(
                    Notification.Types.ChatWork.ToInt().ToString(),
                    Displays.ChatWork(context: context));
            }
            if (Parameters.Notification.Line)
            {
                notificationTypes.Add(
                    Notification.Types.Line.ToInt().ToString(),
                    Displays.Line(context: context));
                notificationTypes.Add(
                    Notification.Types.LineGroup.ToInt().ToString(),
                    Displays.LineGroup(context: context));
            }
            if (Parameters.Notification.Teams)
            {
                notificationTypes.Add(
                    Notification.Types.Teams.ToInt().ToString(),
                    Displays.Teams(context: context));
            }
            if (Parameters.Notification.RocketChat)
            {
                notificationTypes.Add(
                    Notification.Types.RocketChat.ToInt().ToString(),
                    Displays.RocketChat(context: context));
            }
            if (Parameters.Notification.InCircle)
            {
                notificationTypes.Add(
                    Notification.Types.InCircle.ToInt().ToString(),
                    Displays.InCircle(context: context));
            }
            if (Parameters.Notification.HttpClient && !isProcessNotification)
            {
                notificationTypes.Add(
                    Notification.Types.HttpClient.ToInt().ToString(),
                    Displays.HttpClient(context: context));
            }
            return notificationTypes;
        }

        public static Dictionary<string, string> OrderTypes(Context context, bool isProcessNotification=false)
        {
            var orderTypes = Parameters.Notification.ListOrder?
                .Select(o => System.Enum.TryParse(o, out Notification.Types t)
                    ? t
                    : (Notification.Types?)null)
                .Where(o => o != null);
            var notificationTypes = new Dictionary<string, string>();
            if (orderTypes == null)
            {
                return notificationTypes;
            }
            foreach (var type in orderTypes)
            {
                switch (type)
                {
                    case Notification.Types.Mail:
                        if (Parameters.Notification.Mail)
                        {
                            notificationTypes.Add(
                                type.ToInt().ToString(),
                                Displays.Mail(context: context));
                        }
                        break;
                    case Notification.Types.Slack:
                        if (Parameters.Notification.Slack)
                        {
                            notificationTypes.Add(
                                type.ToInt().ToString(),
                                Displays.Slack(context: context));
                        }
                        break;
                    case Notification.Types.ChatWork:
                        if (Parameters.Notification.ChatWork)
                        {
                            notificationTypes.Add(
                                type.ToInt().ToString(),
                                Displays.ChatWork(context: context));
                        }
                        break;
                    case Notification.Types.Line:
                        if (Parameters.Notification.Line)
                        {
                            notificationTypes.Add(
                                type.ToInt().ToString(),
                                Displays.Line(context: context));
                        }
                        break;
                    case Notification.Types.LineGroup:
                        if (Parameters.Notification.Line)
                        {
                            notificationTypes.Add(
                                type.ToInt().ToString(),
                                Displays.LineGroup(context: context));
                        }
                        break;
                    case Notification.Types.Teams:
                        if (Parameters.Notification.Teams)
                        {
                            notificationTypes.Add(
                                type.ToInt().ToString(),
                                Displays.Teams(context: context));
                        }
                        break;
                    case Notification.Types.RocketChat:
                        if (Parameters.Notification.RocketChat)
                        {
                            notificationTypes.Add(
                                type.ToInt().ToString(),
                                Displays.RocketChat(context: context));
                        }
                        break;
                    case Notification.Types.InCircle:
                        if (Parameters.Notification.InCircle)
                        {
                            notificationTypes.Add(
                                type.ToInt().ToString(),
                                Displays.InCircle(context: context));
                        }
                        break;
                    case Notification.Types.HttpClient:
                        if (Parameters.Notification.HttpClient && !isProcessNotification)
                        {
                            notificationTypes.Add(
                                type.ToInt().ToString(),
                                Displays.HttpClient(context: context));
                        }
                        break;
                }
            }
            return notificationTypes;
        }

        public static Dictionary<string, string> MethodTypes(Context context)
        {
            var methodTypes = new Dictionary<string, string>();

            var types = System.Enum.GetValues(typeof(Notification.MethodTypes));
            foreach (var t in types)
            {
                methodTypes.Add(t.ToInt().ToString(), t.ToString());
            }
            return methodTypes;
        }

        public static Dictionary<string, string> Encodings(Context context)
        {
            return Parameters.Notification.HttpClientEncodings?
                .ToDictionary(k => k, v => v);
        }

        public static bool NotificationType(Notification notification)
        {
            return notification.Type == Notification.Types.HttpClient;
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
                Notification.Types.LineGroup,
                Notification.Types.InCircle
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