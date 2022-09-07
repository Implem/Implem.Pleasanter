using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class ToApiValueExtensions
    {
        public static object ToApiValue(
            this bool self,
            Context context,
            SiteSettings ss,
            Column column)
        {
            return self;
        }

        public static object ToApiValue(
            this bool? self,
            Context context,
            SiteSettings ss,
            Column column)
        {
            return self;
        }

        public static object ToApiValue(
            this DateTime self,
            Context context,
            SiteSettings ss,
            Column column)
        {
            return self.ToLocal(context: context);
        }

        public static object ToApiValue(
            this string self,
            Context context,
            SiteSettings ss,
            Column column,
            string delimiter = ", ",
            ExportColumn.Types type = ExportColumn.Types.Text)
        {
            return self;
        }

        public static object ToApiValue(
            this int self,
            Context context,
            SiteSettings ss,
            Column column)
        {
            return self;
        }

        public static object ToApiValue(
            this long self,
            Context context,
            SiteSettings ss,
            Column column)
        {
            return self;
        }

        public static object ToApiValue(
            this double self,
            Context context,
            SiteSettings ss,
            Column column)
        {
            return self;
        }

        public static object ToApiValue(
            this TimeZoneInfo self,
            Context context,
            SiteSettings ss,
            Column column)
        {
            return self.Id;
        }

        public static object ToApiValue(
            this Enum self,
            Context context,
            SiteSettings ss,
            Column column)
        {
            return self;
        }
    }
}