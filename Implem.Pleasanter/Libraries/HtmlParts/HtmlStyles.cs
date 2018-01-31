using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlStyles
    {
        public static HtmlBuilder Styles(
            this HtmlBuilder hb, SiteSettings ss, string userStyle)
        {
            return hb
                .Style(
                    style: ss.GetStyleBody(o => o.All == true),
                    _using: Contract.Style() && ss?.Styles?.Any() == true)
                .Style(
                    style: userStyle,
                    _using: Contract.Style() && !userStyle.IsNullOrEmpty());
        }
    }
}