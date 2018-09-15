using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class ToResponseExtensions
    {
        public static string ToResponse(
            this Enum self, Context context, SiteSettings ss, Column column)
        {
            return self.ToString();
        }

        public static string ToResponse(
            this bool self, Context context, SiteSettings ss, Column column)
        {
            return self.ToString();
        }

        public static string ToResponse(
            this DateTime self, Context context, SiteSettings ss, Column column)
        {
            return self.InRange()
                ? column.DisplayControl(self.ToLocal())
                : string.Empty;
        }

        public static string ToResponse(
            this int self, Context context, SiteSettings ss, Column column)
        {
            return self.ToString();
        }

        public static string ToResponse(
            this long self, Context context, SiteSettings ss, Column column)
        {
            return self.ToString();
        }

        public static string ToResponse(
            this decimal self, Context context, SiteSettings ss, Column column)
        {
            return column.ControlType == "Spinner"
                ? column.Display(self, format: false)
                : column.Display(ss, self, format: column.Format == "C");
        }

        public static string ToResponse(
            this string self, Context context, SiteSettings ss, Column column)
        {
            return column.EditorReadOnly != true || !column.HasChoices()
                ? self.ToString()
                : column.Choice(self).Text;
        }
    }
}