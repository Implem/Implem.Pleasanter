using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlStyles
    {
        public static string Get()
        {
            return new HtmlBuilder()
                .Link(
                    rel: "stylesheet",
                    href: Locations.Get("Resources/Styles?v=" +
                        Parameters.ExtendedStyles.Join().Sha512Cng()),
                    _using: Parameters.ExtendedStyles?.Any() == true)
                .ToString();
        }

        public static HtmlBuilder Styles(
            this HtmlBuilder hb, SiteSettings ss = null, string userStyle = null)
        {
            return hb
                .Style(
                    style: ss?.GetStyleBody(o => o.All == true),
                    _using: Contract.Style() && ss?.Styles?.Any() == true)
                .Style(
                    style: userStyle,
                    _using: Contract.Style() && !userStyle.IsNullOrEmpty());
        }
    }
}