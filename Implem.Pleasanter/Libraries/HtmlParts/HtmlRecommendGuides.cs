using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Text.RegularExpressions;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlRecommendGuides
    {
        private static readonly Regex ItemPathRegex =
            new Regex(@"^/items/\d+(/edit)?$", RegexOptions.Compiled);

        public static HtmlBuilder RecommendGuide(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss)
        {
            var isRecommendGuidePath = IsRecommendGuidePath(
                context: context,
                ss: ss);
            var isRecommendGuideShow = IsRecommendGuideShow();
            var isRecommendGuide = isRecommendGuidePath && isRecommendGuideShow;
            var (recommendGuideText, recommendGuideHref) = GetRecommendGuide(context: context);
            return hb.Div(
                id: "RecommendGuide",
                action: () => hb.Div(
                    css: "recommend-guide-inner",
                    action: () => hb.A(
                        href: recommendGuideHref,
                        target: "_blank",
                        action: () => hb.Text(text: recommendGuideText))
                    .Span(
                        css: "material-symbols-sharp is-fill display-none-btn",
                        action: () => hb.Text(text: "close"))),
                _using: isRecommendGuide);
        }

        private static (string text, string href) GetRecommendGuide(Context context)
        {
            string text = Displays.RecommendEnterpriseService(context);
            string href = Parameters.General.RecommendUrl1.Params(
                Parameters.General.PleasanterSource,
                "manual",
                "table-management-reco-guide");
            return (text, href);
        }

        private static bool IsRecommendGuidePath(
            Context context,
            SiteSettings ss)
        {
            const string editAction = "edit";
            if (context.Action != editAction ||
                context.RecordTitle != null ||
                !ss.IsTable())
            {
                return false;
            }
            return ItemPathRegex.IsMatch(context.AbsolutePath);
        }

        private static bool IsRecommendGuideShow()
        {
            return !Parameters.DisableAds()
                && (!Parameters.CommercialLicense() || Parameters.Service.Demo);
        }
    }
}