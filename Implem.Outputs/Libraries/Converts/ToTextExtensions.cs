using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Settings;
using System;
namespace Implem.Pleasanter.Libraries.Converts
{
    public static class ToTextExtensions
    {
        public static string ToText(this DateTime self, Column column)
        {
            return self.NotZero()
                ? self.Formatted(column)
                : string.Empty;
        }

        private static string Formatted(this DateTime self, Column column)
        {
            return column.GridDateTime != string.Empty
                ? self.ToString(
                    Displays.Get(column.GridDateTime + "Format"),
                    Sessions.CultureInfo())
                : self.ToString();
        }
    }
}