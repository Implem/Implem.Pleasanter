using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlHeaders
    {
        public static HtmlBuilder Header(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long siteId,
            string referenceType,
            Error.Types errorType,
            bool useNavigationMenu,
            bool useSearch)
        {
            var title = Title(context: context);
            return hb.Header(id: "Header", action: () => hb
                .H(number: 2, id: "Logo", action: () => hb
                    .A(
                        attributes: new HtmlAttributes().Href(Locations.Top(context: context)),
                        action: () => hb
                            .Img(
                                id: "CorpLogo",
                                src: Locations.Images(
                                    context: context,
                                    parts: title.IsNullOrEmpty()
                                        ? "logo-corp-with-title.png"
                                        : "logo-corp.png"))
                            .Span(id: "ProductLogo", action: () => hb
                                .Text(text: title))))
                .NavigationMenu(
                    context: context,
                    ss: ss,
                    siteId: siteId,
                    referenceType: referenceType,
                    errorType: errorType,
                    useNavigationMenu: useNavigationMenu,
                    useSearch: useSearch));
        }

        private static string Title(Context context)
        {
            if (context.Authenticated && Parameters.Service.ShowTenantTitle)
            {
                var title = Rds.ExecuteScalar_string(
                    context: context,
                    statements: Rds.SelectTenants(
                        column: Rds.TenantsColumn().Title(),
                        where: Rds.TenantsWhere().TenantId(context.TenantId)));
                return !title.IsNullOrEmpty()
                    ? title
                    : (Parameters.General.DisplayLogoText)? Displays.ProductName(context: context) : string.Empty;
            }
            else
            {
                return (Parameters.General.DisplayLogoText) ? Displays.ProductName(context: context) : string.Empty;
            }
        }
        
    }
}