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
            bool isSearch,
            ServerScriptModelRow serverScriptModelRow)
        {
            return hb.Announcement(context: context)
                .Header(
                    id: "Header",
                    action: () => hb
                        .HeaderLogo(
                            context: context,
                            ss: ss,
                            _using: context.ThemeVersionOver2_0() && context.Action == "login"
                                ? false
                                : true,
                            isSearch: isSearch)
                        .NavigationMenu(
                            context: context,
                            ss: ss,
                            siteId: siteId,
                            referenceType: referenceType,
                            errorType: errorType,
                            useNavigationMenu: useNavigationMenu,
                            useSearch: useSearch,
                            isSearch: isSearch,
                            serverScriptModelRow: serverScriptModelRow));
        }

        public static HtmlBuilder Announcement(this HtmlBuilder hb, Context context)
        {
            var siteId = Parameters.Service.AnnouncementSiteId;
            if (siteId > 0)
            {
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
                hb.Div(
                    attributes: new HtmlAttributes()
                        .Id("AnnouncementModule")
                        .Class("announcements"),
                    action: () => issueCollection.ForEach(issueModel =>
                    {
                        if (!IsHiddenAnnouncement(
                            context: context,
                            issueModel: issueModel))
                        {
                            hb.Div(
                                attributes: new HtmlAttributes()
                                    .Id($"AnnouncementContainer_{issueModel.IssueId}")
                                    .Class("annonymous", _using: !context.Authenticated),
                                action: () => hb.Raw(text: issueModel.Body));
                        }
                    }));
            }
            return hb;
        }

        private static bool IsHiddenAnnouncement(
            Context context,
            IssueModel issueModel)
        {
            var hideOther = issueModel.CheckHash.TryGetValue("CheckA", out bool checkA) ? checkA : false;
            var hideLogin = issueModel.CheckHash.TryGetValue("CheckB", out bool checkB) ? checkB : false;
            var hideTop = issueModel.CheckHash.TryGetValue("CheckC", out bool checkC) ? checkC : false;
            var allowClose = issueModel.CheckHash.TryGetValue("CheckD", out bool checkD) ? checkD : false;
            var isLogin = context.Controller.ToLower() == "users" && context.Action.ToLower() == "login";
            var isTop = context.Controller.ToLower() == "items" && context.Id == 0;
            if (allowClose)
            {
                if (context.SessionData.ContainsKey($"ClosedAnnouncement:{issueModel.IssueId}"))
                {
                    return true;
                }
            }
            return isLogin && hideLogin
                || isTop && hideTop
                || !isLogin && !isTop && hideOther;
        }

        public static HtmlBuilder HeaderLogo(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            bool _using = true,
            bool isSearch = false)
        {
            var existsImage = BinaryUtilities.ExistsTenantImage(
                context: context,
                ss: ss,
                referenceId: context.TenantId,
                sizeType: Images.ImageData.SizeTypes.Logo,
                isSearch: isSearch);
            var title = Title(context: context);
            return _using
                ? hb.H(number: 2, id: "Logo", action: () => hb
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
                            .Text(text: title))))
                : hb;
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
                            BinaryUtilities.TenantImageUpdatedTime(
                                context,
                                SiteSettingsUtilities.TenantsSiteSettings(context),
                                context.TenantId,
                                Images.ImageData.SizeTypes.Logo)
                                .ToString("?yyyyMMddHHmmss")
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