using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using System;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class FormatExtension
    {
        public static string Display(this DateTime value, string format)
        {
            return value.InRange()
                ? !format.IsNullOrEmpty()
                    ? value.ToString(
                        Displays.Get(format + "Format"),
                        Sessions.CultureInfo())
                    : value.ToString(Sessions.CultureInfo())
                : string.Empty;
        }
    }
}