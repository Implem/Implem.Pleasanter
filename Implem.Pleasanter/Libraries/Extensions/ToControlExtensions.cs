using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class ToControlExtensions
    {
        public static string ToControl(
            this bool self, Context context, SiteSettings ss, Column column)
        {
            return self.ToString();
        }

        public static string ToControl(
            this DateTime self, Context context, SiteSettings ss, Column column)
        {
            return column.DisplayControl(self.ToLocal());
        }

        public static string ToControl(
            this string self, Context context, SiteSettings ss, Column column)
        {
            return self;
        }

        public static string ToControl(
            this int self, Context context, SiteSettings ss, Column column)
        {
            return self.ToString(column.StringFormat);
        }

        public static string ToControl(
            this long self, Context context, SiteSettings ss, Column column)
        {
            return self.ToString(column.StringFormat);
        }

        public static string ToControl(
            this decimal self, Context context, SiteSettings ss, Column column)
        {
            return column.ControlType == "Spinner"
                ? column.Display(self, format: false)
                : column.Display(ss, self, format: column.Format == "C");
        }
    }
}