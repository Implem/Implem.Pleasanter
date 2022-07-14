using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class ReminderUtilities
    {
        public static Dictionary<string, string> Types(Context context)
        {
            return Parameters.Reminder.ListOrder == null
                ? DefaultTypes(context: context)
                : OrderTypes(context: context);
        }

        private static Dictionary<string, string> DefaultTypes(Context context)
        {
            var reminderTypes = new Dictionary<string, string>();
            if (Parameters.Reminder.Mail)
            {
                reminderTypes.Add(
                    Reminder.ReminderTypes.Mail.ToInt().ToString(),
                    Displays.Mail(context: context));
            }
            if (Parameters.Reminder.Slack)
            {
                reminderTypes.Add(
                    Reminder.ReminderTypes.Slack.ToInt().ToString(),
                    Displays.Slack(context: context));
            }
            if (Parameters.Reminder.ChatWork)
            {
                reminderTypes.Add(
                    Reminder.ReminderTypes.ChatWork.ToInt().ToString(),
                    Displays.ChatWork(context: context));
            }
            if (Parameters.Reminder.Line)
            {
                reminderTypes.Add(
                    Reminder.ReminderTypes.Line.ToInt().ToString(),
                    Displays.Line(context: context));
                reminderTypes.Add(
                    Reminder.ReminderTypes.LineGroup.ToInt().ToString(),
                    Displays.LineGroup(context: context));
            }
            if (Parameters.Reminder.Teams)
            {
                reminderTypes.Add(
                    Reminder.ReminderTypes.Teams.ToInt().ToString(),
                    Displays.Teams(context: context));
            }
            if (Parameters.Reminder.RocketChat)
            {
                reminderTypes.Add(
                    Reminder.ReminderTypes.RocketChat.ToInt().ToString(),
                    Displays.RocketChat(context: context));
            }
            if (Parameters.Reminder.InCircle)
            {
                reminderTypes.Add(
                    Reminder.ReminderTypes.InCircle.ToInt().ToString(),
                    Displays.InCircle(context: context));
            }
            return reminderTypes;
        }

        private static Dictionary<string, string> OrderTypes(Context context)
        {
            var orderTypes = Parameters.Reminder.ListOrder?
                .Select(o => System.Enum.TryParse(o, out Reminder.ReminderTypes t)
                    ? t
                    : (Reminder.ReminderTypes?)null)
                .Where(o => o != null);
            var reminderTypes = new Dictionary<string, string>();
            if (orderTypes == null)
            {
                return reminderTypes;
            }
            foreach (var type in orderTypes)
            {
                switch (type)
                {
                    case Reminder.ReminderTypes.Mail:
                        if (Parameters.Reminder.Mail)
                        {
                            reminderTypes.Add(
                                type.ToInt().ToString(),
                                Displays.Mail(context: context));
                        }
                        break;
                    case Reminder.ReminderTypes.Slack:
                        if (Parameters.Reminder.Slack)
                        {
                            reminderTypes.Add(
                                type.ToInt().ToString(),
                                Displays.Slack(context: context));
                        }
                        break;
                    case Reminder.ReminderTypes.ChatWork:
                        if (Parameters.Reminder.ChatWork)
                        {
                            reminderTypes.Add(
                                type.ToInt().ToString(),
                                Displays.ChatWork(context: context));
                        }
                        break;
                    case Reminder.ReminderTypes.Line:
                        if (Parameters.Reminder.Line)
                        {
                            reminderTypes.Add(
                                type.ToInt().ToString(),
                                Displays.Line(context: context));
                        }
                        break;
                    case Reminder.ReminderTypes.LineGroup:
                        if (Parameters.Reminder.Line)
                        {
                            reminderTypes.Add(
                                type.ToInt().ToString(),
                                Displays.LineGroup(context: context));
                        }
                        break;
                    case Reminder.ReminderTypes.Teams:
                        if (Parameters.Reminder.Teams)
                        {
                            reminderTypes.Add(
                                type.ToInt().ToString(),
                                Displays.Teams(context: context));
                        }
                        break;
                    case Reminder.ReminderTypes.RocketChat:
                        if (Parameters.Reminder.RocketChat)
                        {
                            reminderTypes.Add(
                                type.ToInt().ToString(),
                                Displays.RocketChat(context: context));
                        }
                        break;
                    case Reminder.ReminderTypes.InCircle:
                        if (Parameters.Reminder.InCircle)
                        {
                            reminderTypes.Add(
                                type.ToInt().ToString(),
                                Displays.InCircle(context: context));
                        }
                        break;
                }
            }
            return reminderTypes;
        }

        public static bool RequireFrom(Reminder reminder)
        {
            return FromListItems().Contains(reminder.ReminderType);
        }

        public static string FromList()
        {
            return FromListItems()
                .Select(o => o.ToInt()
                .ToString())
                .Join();
        }

        private static List<Reminder.ReminderTypes> FromListItems()
        {
            return new List<Reminder.ReminderTypes>
            {
                Reminder.ReminderTypes.Mail,
                Reminder.ReminderTypes.Slack,
                Reminder.ReminderTypes.ChatWork,
                Reminder.ReminderTypes.RocketChat,
                Reminder.ReminderTypes.InCircle
            }
                .Where(o => Parameters.Mail.FixedFrom.IsNullOrEmpty()
                    || o != Reminder.ReminderTypes.Mail)
                .ToList();
        }

        public static bool RequireToken(Reminder reminder)
        {
            return TokenListItems().Contains(reminder.ReminderType);
        }

        public static string TokenList()
        {
            return TokenListItems()
                .Select(o => o.ToInt()
                .ToString())
                .Join();
        }

        private static List<Reminder.ReminderTypes> TokenListItems()
        {
            return new List<Reminder.ReminderTypes>
            {
                Reminder.ReminderTypes.ChatWork,
                Reminder.ReminderTypes.Line,
                Reminder.ReminderTypes.LineGroup,
                Reminder.ReminderTypes.InCircle
            };  
        }
    }
}