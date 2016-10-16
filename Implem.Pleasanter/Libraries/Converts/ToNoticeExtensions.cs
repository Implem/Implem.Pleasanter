using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.Converts
{
    public static class ToNoticeExtensions
    {
        public static string ToNotice(
            this bool self,
            bool saved,
            Column column,
            bool updated,
            bool update)
        {
            return self.ToString().ToNotice(
                saved.ToString(),
                column,
                updated,
                update);
        }

        public static string ToNotice(
            this int self,
            int saved,
            Column column,
            bool updated,
            bool update)
        {
            return self.ToString().ToNotice(
                saved.ToString(),
                column,
                updated,
                update);
        }

        public static string ToNotice(
            this long self,
            long saved,
            Column column,
            bool updated,
            bool update)
        {
            return self.ToString().ToNotice(
                saved.ToString(),
                column,
                updated,
                update);
        }

        public static string ToNotice(
            this decimal self,
            decimal saved,
            Column column,
            bool updated,
            bool update)
        {
            return column.Display(self, unit: true).ToNotice(
                column.Display(saved, unit: true),
                column,
                updated,
                update);
        }

        public static string ToNotice(
            this DateTime self,
            DateTime saved,
            Column column,
            bool updated,
            bool update)
        {
            return column.DisplayControl(self.ToLocal()).ToNotice(
                column.DisplayControl(saved.ToLocal()),
                column,
                updated,
                update);
        }

        public static string ToNotice(
            this string self,
            string saved,
            Column column,
            bool updated,
            bool update)
        {
            return update
                ? updated
                    ? saved != string.Empty
                        ? "{0} : {2} => {1}\n".Params(column.LabelText, self, saved)
                        : "{0} : {1}\n".Params(column.LabelText, self)
                    : string.Empty
                : "{0}: {1}\n".Params(column.LabelText, self);
        }
    }
}