using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using System;
namespace Implem.Pleasanter.Libraries.Extensions
{
    public static class FormatExtension
    {
        public static string Display(this DateTime value, Context context, string format)
        {
            return value.InRange()
                ? !format.IsNullOrEmpty()
                    ? value.ToString(
                        Displays.Get(
                            context: context,
                            id: format + "Format"),
                        context.CultureInfo())
                    : value.ToString(context.CultureInfo())
                : string.Empty;
        }
    }
}