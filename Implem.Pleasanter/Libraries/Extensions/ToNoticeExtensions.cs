using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
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
            this decimal self,
            Context context,
            decimal saved,
            Column column,
            NotificationColumnFormat notificationColumnFormat,
            bool updated,
            bool update)
        {
            return notificationColumnFormat.DisplayText(
                self: column.Display(
                    context: context,
                    value: self,
                    unit: true),
                saved: column.Display(
                    context: context,
                    value: saved,
                    unit: true),
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
                switch (column.Type)
                {
                    case Column.Types.Dept:
                        return notificationColumnFormat.DisplayText(
                            self: SiteInfo.Dept(
                                tenantId: context.TenantId,
                                deptId: self.ToInt()).Name,
                            saved: SiteInfo.Dept(
                                tenantId: context.TenantId,
                                deptId: saved.ToInt()).Name,
                            column: column,
                            updated: updated,
                            update: update);
                    case Column.Types.Group:
                        return notificationColumnFormat.DisplayText(
                            self: SiteInfo.Group(
                                tenantId: context.TenantId,
                                groupId: self.ToInt()).Name,
                            saved: SiteInfo.Group(
                                tenantId: context.TenantId,
                                groupId: saved.ToInt()).Name,
                            column: column,
                            updated: updated,
                            update: update);
                    case Column.Types.User:
                        return notificationColumnFormat.DisplayText(
                            self: SiteInfo.UserName(
                                context: context,
                                userId: self.ToInt()),
                            saved: SiteInfo.UserName(
                                context: context,
                                userId: saved.ToInt()),
                            column: column,
                            updated: updated,
                            update: update);
                    default:
                        return notificationColumnFormat.DisplayText(
                    self: column.Choice(self).Text,
                    saved: column.Choice(saved).Text,
                    column: column,
                    updated: updated,
                            update: update);
                }
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