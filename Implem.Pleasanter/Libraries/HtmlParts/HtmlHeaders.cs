using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
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
            return hb.Header(id: "Header", action: () => hb
                .HeaderLogo(context)
                .NavigationMenu(
                    context: context,
                    ss: ss,
                    siteId: siteId,
                    referenceType: referenceType,
                    errorType: errorType,
                    useNavigationMenu: useNavigationMenu,
                    useSearch: useSearch));
        }

        public static HtmlBuilder HeaderLogo(this HtmlBuilder hb, Context context)
        {
            var ss = SiteSettingsUtilities.TenantsSiteSettings(context);
            var existsImage = BinaryUtilities.ExistsTenantImage(
                context: context,
                ss: ss,
                referenceId: context.TenantId,
                sizeType: Images.ImageData.SizeTypes.Logo);
            var title = Title(context: context);
            return hb.H(number: 2, id: "Logo", action: () => hb
                .A(
                    attributes: new HtmlAttributes().Href(context.Publish
                        ? Locations.ItemIndex(
                            context: context,
                            id: context.SiteId)
                        : Locations.Top(context: context)),
                    action: () => hb
                        .LogoImage(
                            context: context,
                            showTitle: !title.IsNullOrEmpty(),
                            existsTenantImage: existsImage)
                        .Span(id: "ProductLogo", action: () => hb
                        .Text(text: title))));
        }

        private static HtmlBuilder LogoImage(
            this HtmlBuilder hb, Context context, bool showTitle, bool existsTenantImage)
        {
            return existsTenantImage
                ? hb.Img(
                    id: "CorpLogo",
                    src: Locations.Get(
                        context: context,
                        parts: new string[]
                        {
                            "Binaries",
                            "TenantImageLogo",
                            BinaryUtilities.TenantImagePrefix(
                                context,
                                SiteSettingsUtilities.TenantsSiteSettings(context),
                                context.TenantId,
                                Images.ImageData.SizeTypes.Logo)
                        }))
                : hb.Img(
                    id: "CorpLogo",
                    src: Locations.Images(
                        context: context,
                        parts: showTitle
                            ? "logo-corp.png"
                            : "logo-corp-with-title.png"));
        }

        private static string Title(Context context)
        {
            return Strings.CoalesceEmpty(
                context.LogoType == TenantModel.LogoTypes.ImageAndTitle
                    ? context.TenantTitle
                    : string.Empty,
                Parameters.General.DisplayLogoText
                     ? Displays.ProductName(context: context)
                     : string.Empty);
        }
    }
}