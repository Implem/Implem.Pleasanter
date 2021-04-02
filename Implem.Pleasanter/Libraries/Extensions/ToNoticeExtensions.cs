using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Linq;
using System.Text;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class ToNoticeExtensions
    {
        public static string ToNotice(
            this bool self,
            Context context,
            bool saved,
            Column column,
            NotificationColumnFormat notificationColumnFormat,
            bool updated,
            bool update)
        {
            return notificationColumnFormat.DisplayText(
                self: self.ToString(),
                saved: saved.ToString(),
                column: column,
                updated: updated,
                update: update);
        }

        public static string ToNotice(
            this int self,
            Context context,
            int saved,
            Column column,
            NotificationColumnFormat notificationColumnFormat,
            bool updated,
            bool update)
        {
            return notificationColumnFormat.DisplayText(
                self: self.ToString(),
                saved: saved.ToString(),
                column: column,
                updated: updated,
                update: update);
        }

        public static string ToNotice(
            this long self,
            Context context,
            long saved,
            Column column,
            NotificationColumnFormat notificationColumnFormat,
            bool updated,
            bool update)
        {
            return notificationColumnFormat.DisplayText(
                self: self.ToString(),
                saved: saved.ToString(),
                column: column,
                updated: updated,
                update: update);
        }

        public static string ToNotice(
            this DateTime self,
            Context context,
            DateTime saved,
            Column column,
            NotificationColumnFormat notificationColumnFormat,
            bool updated,
            bool update)
        {
            return notificationColumnFormat.DisplayText(
                self: column.DisplayControl(
                    context: context,
                    value: self.ToLocal(context: context)),
                saved: column.DisplayControl(
                    context: context,
                    value: saved.ToLocal(context: context)),
                column: column,
                updated: updated,
                update: update);
        }

        public static string ToNotice(
            this string self,
            Context context,
            string saved,
            Column column,
            NotificationColumnFormat notificationColumnFormat,
            bool updated,
            bool update)
        {
            if (column.HasChoices())
            {
                var selfChoiceParts = column.ChoiceParts(
                    context: context,
                    selectedValues: self,
                    type: ExportColumn.Types.Text);
                var savedChoiceParts = column.ChoiceParts(
                    context: context,
                    selectedValues: saved,
                    type: ExportColumn.Types.Text);
                return notificationColumnFormat.DisplayText(
                    self: column.MultipleSelections == true
                        ? selfChoiceParts.Join(", ")
                        : selfChoiceParts.FirstOrDefault(),
                    saved: column.MultipleSelections == true
                        ? savedChoiceParts.Join(", ")
                        : savedChoiceParts.FirstOrDefault(),
                    column: column,
                    updated: updated,
                    update: update);
            }
            else
            {
                return notificationColumnFormat.DisplayText(
                    self: self,
                    saved: saved,
                    column: column,
                    updated: updated,
                    update: update);
            }
        }

        public static string ToNotice(
            this Attachments self,
            Context context,
            string saved,
            Column column,
            NotificationColumnFormat notificationColumnFormat,
            bool updated,
            bool update)
        {
            var body = new StringBuilder();
            body.Append("\n");
            self.ForEach(attachment =>
            {
                if (attachment.Added == true)
                {
                    body.Append("  {0} - {1}\n".Params(
                        Displays.Add(context: context),
                        attachment.Name));
                }
                else if (attachment.Deleted == true)
                {
                    body.Append("  {0} - {1}\n".Params(
                        Displays.Delete(context: context),
                        attachment.Name));
                }
            });
            return notificationColumnFormat.DisplayText(
                self: body.ToString(),
                saved: null,
                column: column,
                updated: updated && !body.ToString().IsNullOrEmpty(),
                update: update);
        }
    }
}