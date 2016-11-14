using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Images;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlTemplates
    {
        public static HtmlBuilder Template(
            this HtmlBuilder hb,
            Permissions.Types pt,
            Versions.VerTypes verType,
            BaseModel.MethodTypes methodType,
            bool allowAccess,
            long siteId = 0,
            long parentId = 0,
            string referenceType = "",
            string siteReferenceType = "",
            string title = "",
            bool useBreadcrumb = true,
            bool useTitle = true,
            bool useSearch = true,
            bool useNavigationMenu = true,
            string script = "",
            string userScript = "",
            string userStyle = "",
            Action action = null)
        {
            return hb
                .MainContainer(
                    pt: pt,
                    verType: verType,
                    methodType: methodType,
                    allowAccess: allowAccess,
                    siteId: siteId,
                    parentId: parentId,
                    referenceType: referenceType,
                    siteReferenceType: siteReferenceType,
                    title: title,
                    useBreadcrumb: useBreadcrumb,
                    useTitle: useTitle,
                    useSearch: useSearch,
                    useNavigationMenu: useNavigationMenu,
                    userStyle: userStyle,
                    action: action)
                .HiddenData()
                .Scripts(
                    script: script,
                    userScript: userScript,
                    referenceType: referenceType);
        }

        public static HtmlBuilder MainContainer(
            this HtmlBuilder hb,
            Permissions.Types pt,
            Versions.VerTypes verType,
            BaseModel.MethodTypes methodType,
            bool allowAccess,
            long siteId,
            long parentId,
            string referenceType,
            string siteReferenceType,
            string title,
            bool useBreadcrumb = true,
            bool useTitle = true,
            bool useSearch = true,
            bool useNavigationMenu = true,
            string userStyle = "",
            Action action = null)
        {
            return hb.Div(id: "MainContainer", action: () => hb
                .Header(
                    pt: pt,
                    siteId: siteId,
                    referenceType: referenceType,
                    allowAccess: allowAccess,
                    useNavigationMenu: useNavigationMenu,
                    useSearch: useSearch)
                .Content(
                    pt: pt,
                    allowAccess: allowAccess,
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
                    siteReferenceType: siteReferenceType)
                .Styles(style: userStyle));
        }

        private static HtmlBuilder Content(
            this HtmlBuilder hb,
            Permissions.Types pt,
            bool allowAccess,
            long siteId = 0,
            string title = "",
            bool useBreadcrumb = true,
            bool useTitle = true,
            Action action = null)
        {
            return hb.Article(id: "Application", action: () =>
            {
                if (allowAccess)
                {
                    hb.Nav(css: "both cf", action: () => hb
                        .Breadcrumb(
                            siteId: siteId,
                            pt: pt,
                            _using: useBreadcrumb));
                    if (useTitle)
                    {
                        hb.Title(pt: pt, siteId: siteId, text: title);
                    }
                    action();
                    hb.P(id: "Message", css: "message", action: () => hb
                        .Raw(text: Sessions.Message()));
                }
                else
                {
                    hb
                        .P(id: "Message", css: "message", action: () => hb
                            .Raw(text: Messages.NotFound().Html))
                        .MainCommands(
                            siteId: siteId,
                            pt: pt,
                            verType: Versions.VerTypes.Latest);
                }
            });
        }

        private static HtmlBuilder Title(
            this HtmlBuilder hb, Permissions.Types pt, long siteId, string text)
        {
            if (text != string.Empty)
            {
                if (BinaryUtilities.ExistsSiteImage(
                    pt, siteId, ImageData.SizeTypes.Icon))
                {
                    hb.Img(
                        src: Locations.Get(
                            "Items",
                            siteId.ToString(),
                            "Binaries",
                            "SiteImageIcon",
                            BinaryUtilities.SiteImagePrefix(
                                pt, siteId, ImageData.SizeTypes.Icon)),
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

        public static HtmlBuilder NotFoundTemplate(this HtmlBuilder hb)
        {
            return hb.Template(
                pt: Permissions.Types.NotSet,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.NotSet,
                allowAccess: false,
                useBreadcrumb: false,
                useTitle: false,
                useNavigationMenu: false);
        }
    }
}