using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlHeaders
    {
        public static HtmlBuilder Header(
            this HtmlBuilder hb,
            SiteSettings ss,
            long siteId,
            string referenceType,
            Error.Types errorType,
            bool useNavigationMenu,
            bool useSearch)
        {
            return hb.Header(id: "Header", action: () => hb
                .H(number: 2, id: "Logo", action: () => hb
                    .A(
                        attributes: new HtmlAttributes().Href(Locations.Top()),
                        action: () => hb
                            .Img(
                                id: "CorpLogo",
                                src: Locations.Images("logo-corp.png"))
                            .Span(id: "ProductLogo", action: () => hb
                                .Text(text: Title()))))
                .NavigationMenu(
                    ss: ss,
                    siteId: siteId,
                    referenceType: referenceType,
                    errorType: errorType,
                    useNavigationMenu: useNavigationMenu,
                    useSearch: useSearch));
        }

        private static string Title()
        {
            if (Parameters.Service.ShowTenantTitle)
            {
                return Rds.ExecuteScalar_string(statements:
                    Rds.SelectTenants(
                        column: Rds.TenantsColumn().Title(),
                        where: Rds.TenantsWhere().TenantId(Sessions.TenantId())));
            }
            else
            {
                return Displays.ProductName();
            }
        }
    }
}