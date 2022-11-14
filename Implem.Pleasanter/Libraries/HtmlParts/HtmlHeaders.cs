using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
using System;

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
            bool useSearch,
            ServerScriptModelRow serverScriptModelRow)
        {
            return hb.Header(id: "Header", action: () => hb
                .Announcement()
                .HeaderLogo(
                    context: context,
                    ss: ss)
                .NavigationMenu(
                    context: context,
                    ss: ss,
                    siteId: siteId,
                    referenceType: referenceType,
                    errorType: errorType,
                    useNavigationMenu: useNavigationMenu,
                    useSearch: useSearch,
                    serverScriptModelRow: serverScriptModelRow));
        }

        public static HtmlBuilder Announcement(this HtmlBuilder hb)
        {
            var siteId = Parameters.Service.AnnouncementSiteId;
            if (siteId > 0)
            {
                var context = new Context(
                    sessionStatus: false,
                    sessionData: false,
                    item: false,
                    setPermissions: false);
                var ss = SiteSettingsUtilities.Get(
                    context: context,
                    siteId: siteId);
                var now = DateTime.Now;
                var issueCollection = new IssueCollection(
                    context: context,
                    ss: ss,
                    where: Rds.IssuesWhere()
                        .SiteId(Parameters.Service.AnnouncementSiteId)
                        .Status(_operator: $"<{Parameters.General.CompletionCode}")
                        .StartTime(now, _operator: "<=")
                        .CompletionTime(now, _operator: ">="));
                foreach (var issueModel in issueCollection)
                {
                    hb.Raw(text: issueModel.Body);
                }
            }
            return hb;
        }

        public static HtmlBuilder HeaderLogo(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss)
        {
            var existsImage = BinaryUtilities.ExistsTenantImage(
                context: context,
                ss: ss,
                referenceId: context.TenantId,
                sizeType: Images.ImageData.SizeTypes.Logo);
            var title = Title(context: context);
            return hb.H(number: 2, id: "Logo", action: () => hb
                .A(
                    attributes: new HtmlAttributes().Href(context.Publish
                        ? ss.ReferenceType == "Wikis"
                            ? Locations.ItemEdit(
                                context: context,
                                id: context.Id)
                            : Locations.ItemIndex(
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
            return existsTenantImage && !context.Publish
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