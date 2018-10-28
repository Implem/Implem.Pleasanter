using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
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
            this HtmlBuilder hb, Context context, SiteSettings ss, string userStyle = null)
        {
            return hb
                .Style(
                    style: ss.GetStyleBody(context: context, peredicate: o => o.All == true),
                    _using: context.ContractSettings.Style != false
                        && ss.Styles?.Any() == true)
                .Style(
                    style: userStyle,
                    _using: context.ContractSettings.Style != false
                        && !userStyle.IsNullOrEmpty());
        }
    }
}