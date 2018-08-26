using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class ToNoticeExtensions
    {
        public static string ToNotice(
            this bool self,
            Context context,
            bool saved,
            Column column,
            bool updated,
            bool update)
        {
            return self.ToString().ToNoticeLine(
                context: context,
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
            bool updated,
            bool update)
        {
            return self.ToString().ToNoticeLine(
                context: context,
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
            bool updated,
            bool update)
        {
            return self.ToString().ToNoticeLine(
                context: context,
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
            bool updated,
            bool update)
        {
            return column.Display(self, unit: true).ToNoticeLine(
                context: context,
                saved: column.Display(saved, unit: true),
                column: column,
                updated: updated,
                update: update);
        }

        public static string ToNotice(
            this DateTime self,
            Context context,
            DateTime saved,
            Column column,
            bool updated,
            bool update)
        {
            return column.DisplayControl(self.ToLocal()).ToNoticeLine(
                context: context,
                saved: column.DisplayControl(saved.ToLocal()),
                column: column,
                updated: updated,
                update: update);
        }

        public static string ToNotice(
            this string self,
            Context context,
            string saved,
            Column column,
            bool updated,
            bool update)
        {
            return column.HasChoices()
                ? column.Choice(self).Text.ToNoticeLine(
                    context: context,
                    saved: column.Choice(saved).Text,
                    column: column,
                    updated: updated,
                    update: update)
                : self.ToNoticeLine(
                    context: context,
                    saved: saved,
                    column: column,
                    updated: updated,
                    update: update);
        }

        public static string ToNoticeLine(
            this string self,
            Context context,
            string saved,
            Column column,
            bool updated,
            bool update,
            string suffix = null)
        {
            return update
                ? updated
                    ? saved != string.Empty
                        ? "{0}{3} : {2} => {1}\r\n".Params(column.LabelText, self, saved, suffix)
                        : "{0}{2} : {1}\r\n".Params(column.LabelText, self, suffix)
                    : string.Empty
                : "{0}{2}: {1}\r\n".Params(column.LabelText, self, suffix);
        }
    }
}