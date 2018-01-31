using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Images;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlTemplates
    {
        public static HtmlBuilder Template(
            this HtmlBuilder hb,
            SiteSettings ss,
            Versions.VerTypes verType,
            BaseModel.MethodTypes methodType,
            long siteId = 0,
            long parentId = 0,
            string referenceType = null,
            string siteReferenceType = null,
            string title = null,
            bool useBreadcrumb = true,
            bool useTitle = true,
            bool useSearch = true,
            bool useNavigationMenu = true,
            string script = null,
            string userScript = null,
            string userStyle = null,
            Action action = null)
        {
            return hb
                .MainContainer(
                    ss: ss,
                    verType: verType,
                    methodType: methodType,
                    siteId: siteId,
                    parentId: parentId,
                    referenceType: referenceType,
                    siteReferenceType: siteReferenceType,
                    title: title,
                    useBreadcrumb: useBreadcrumb,
                    useTitle: useTitle,
                    useSearch: useSearch,
                    useNavigationMenu: useNavigationMenu,
                    action: action)
                .HiddenData()
                .Styles(ss: ss, userStyle: userStyle)
                .Scripts(
                    ss: ss,
                    script: script,
                    userScript: userScript,
                    referenceType: referenceType);
        }

        public static HtmlBuilder MainContainer(
            this HtmlBuilder hb,
            SiteSettings ss,
            Versions.VerTypes verType,
            BaseModel.MethodTypes methodType,
            long siteId,
            long parentId,
            string referenceType,
            string siteReferenceType,
            string title,
            Error.Types errorType = General.Error.Types.None,
            bool useBreadcrumb = true,
            bool useTitle = true,
            bool useSearch = true,
            bool useNavigationMenu = true,
            Action action = null)
        {
            return hb.Div(id: "MainContainer", action: () => hb
                .Header(
                    ss: ss,
                    siteId: siteId,
                    referenceType: referenceType,
                    errorType: errorType,
                    useNavigationMenu: useNavigationMenu,
                    useSearch: useSearch)
                .Content(
                    ss: ss,
                    errorType: errorType,
                    siteId: siteId,
                    title: title,
                    useBreadcrumb: useBreadcrumb,
                    useTitle: useTitle,
                    action: action)
                .Div(id: "BottomMargin both")
                .Footer()
                .BackUrl(
                    siteId: siteId,
                    parentId: parentId,
                    referenceType: referenceType,
                    siteReferenceType: siteReferenceType));
        }

        private static HtmlBuilder Content(
            this HtmlBuilder hb,
            SiteSettings ss,
            Error.Types errorType,
            long siteId = 0,
            string title = null,
            bool useBreadcrumb = true,
            bool useTitle = true,
            Action action = null)
        {
            return hb.Article(id: "Application", action: () =>
            {
                if (!errorType.Has())
                {
                    hb.Nav(css: "both cf", action: () => hb
                        .Breadcrumb(
                            siteId: siteId,
                            ss: ss,
                            _using: useBreadcrumb));
                    if (useTitle)
                    {
                        hb.Title(ss: ss, siteId: siteId, text: title);
                    }
                    action?.Invoke();
                    hb.P(id: "Message", css: "message", action: () => hb
                        .Raw(text: Sessions.Message()));
                }
                else
                {
                    hb
                        .P(id: "Message", css: "message", action: () => hb
                            .Raw(text: errorType.Message().Html))
                        .MainCommands(
                            siteId: siteId,
                            ss: ss,
                            verType: Versions.VerTypes.Latest);
                }
            });
        }

        private static HtmlBuilder Title(
            this HtmlBuilder hb, SiteSettings ss, long siteId, string text)
        {
            if (!text.IsNullOrEmpty())
            {
                if (BinaryUtilities.ExistsSiteImage(ss, siteId, ImageData.SizeTypes.Icon))
                {
                    hb.Img(
                        src: Locations.Get(
                            "Items",
                            siteId.ToString(),
                            "Binaries",
                            "SiteImageIcon",
                            BinaryUtilities.SiteImagePrefix(
                                ss, siteId, ImageData.SizeTypes.Icon)),
                        css: "site-image-icon");
                }
                return hb.Header(id: "HeaderTitleContainer", action: () => hb
                    .H(number: 1, id: "HeaderTitle", action: () => hb
                        .Text(text: text)));
            }
            else
            {
                return hb;
            }
        }

        private static HtmlBuilder HiddenData(this HtmlBuilder hb)
        {
            return !Request.IsAjax()
                ? hb
                    .Hidden(controlId: "ApplicationPath", value: Locations.Get())
                    .Hidden(controlId: "Language", value: Sessions.Language())
                : hb;
        }

        public static string Error(Error.Types errorType)
        {
            var hb = new HtmlBuilder();
            return hb
                .MainContainer(
                    ss: new SiteSettings(),
                    verType: Versions.VerTypes.Latest,
                    methodType: BaseModel.MethodTypes.NotSet,
                    siteId: 0,
                    parentId: 0,
                    referenceType: null,
                    siteReferenceType: null,
                    title: null,
                    errorType: errorType,
                    useBreadcrumb: false,
                    useTitle: false,
                    useNavigationMenu: true)
                .HiddenData()
                .ToString();
        }
    }
}