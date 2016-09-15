using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
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
            long siteId,
            string referenceType,
            string title,
            Permissions.Types permissionType,
            Versions.VerTypes verType,
            BaseModel.MethodTypes methodType,
            bool allowAccess,
            bool useBreadcrumb = true,
            bool useTitle = true,
            bool useSearch = true,
            bool useNavigationButtons = true,
            string script = "",
            string userScript = "",
            string userStyle = "",
            Action action = null)
        {
            return hb
                .MainContainer(
                    siteId: siteId,
                    referenceId: referenceType,
                    title: title,
                    permissionType: permissionType,
                    verType: verType,
                    methodType: methodType,
                    allowAccess: allowAccess,
                    useBreadcrumb: useBreadcrumb,
                    useTitle: useTitle,
                    useSearch: useSearch,
                    useNavigationButtons: useNavigationButtons,
                    userStyle: userStyle,
                    action: action)
                .Scripts(
                    script: script,
                    userScript: userScript,
                    referenceType: referenceType);
        }

        public static HtmlBuilder MainContainer(
            this HtmlBuilder hb,
            long siteId,
            string referenceId,
            string title,
            Permissions.Types permissionType,
            Versions.VerTypes verType,
            BaseModel.MethodTypes methodType,
            bool allowAccess,
            bool useBreadcrumb = true,
            bool useTitle = true,
            bool useSearch = true,
            bool useNavigationButtons = true,
            string userStyle = "",
            Action action = null)
        {
            return hb.Div(id: "MainContainer", action: () => hb
                .PageHeader()
                .Article(id: "application", action: () =>
                {
                    if (allowAccess)
                    {
                        hb.Nav(css: "both cf", action: () => hb
                            .Breadcrumb(
                                siteId: siteId,
                                permissionType: permissionType,
                                _using: useBreadcrumb)
                            .NavigationFunctions(
                                siteId: siteId,
                                referenceId: referenceId,
                                permissionType: permissionType,
                                useSearch: useSearch,
                                useNavigationButtons: useNavigationButtons));
                        if (useTitle)
                        {
                            hb.Title(permissionType: permissionType, siteId: siteId, text: title);
                        }
                        action();
                        if (verType == Versions.VerTypes.History)
                        {
                            hb.P(id: "Message", css: "message", action: () => hb
                                .Span(css: "alert-information", action: () => hb
                                    .Displays_ReadOnlyBecausePreviousVer()));
                        }
                        else
                        {
                            hb.P(id: "Message", css: "message", action: () => hb
                                .Raw(text: SessionMessage()));
                        }
                    }
                    else
                    {
                        hb
                            .P(id: "Message", css: "message", action: () => hb
                                .Raw(text: Messages.NotFound().Html))
                            .MainCommands(
                                siteId: siteId,
                                permissionType: permissionType,
                                verType: Versions.VerTypes.Latest,
                                backUrl: BackUrl());
                    }
                })
                .Div(css: "margin-bottom both")
                .Footer(css: "footer", action: () => hb
                    .P(action: () => hb
                        .A(
                            attributes: new HtmlAttributes().Href(Parameters.General.HtmlCopyrightUrl),
                            action: () => hb
                                .Raw(Parameters.General.HtmlCopyright.Params(DateTime.Now.Year)))))
                .Hidden(controlId: "ApplicationPath", value: Navigations.Get())
                .Styles(style: userStyle));
        }

        private static HtmlBuilder Title(
            this HtmlBuilder hb,
            Permissions.Types permissionType,
            long siteId,
            string text)
        {
            var binaryModel = new BinaryModel(permissionType, siteId);
            if (binaryModel.ExistsSiteImage(ImageData.SizeTypes.Icon))
            {
                hb.Img(
                    src: Navigations.Get(
                        "Items",
                        siteId.ToString(),
                        "Binaries",
                        "SiteImageIcon",
                        binaryModel.SiteImagePrefix(ImageData.SizeTypes.Icon)),
                    css: "site-image-icon");
            }
            return text != string.Empty
                ? hb.Header(css: "application-title", action: () => hb
                    .H(number: 1, id: "HeaderTitle", action: () => hb
                        .Text(text: text)))
                : hb;
        }

        private static string SessionMessage()
        {
            var html = Sessions.Data("Message");
            if (html != string.Empty)
            {
                Sessions.Clear("Message");
                return html;
            }
            else
            {
                return string.Empty;
            }
        }

        public static HtmlBuilder NotFoundTemplate(this HtmlBuilder hb)
        {
            return hb.Template(
                siteId: 0,
                referenceType: string.Empty,
                title: string.Empty,
                permissionType: Permissions.Types.NotSet,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.NotSet,
                allowAccess: false,
                useBreadcrumb: false,
                useTitle: false,
                useNavigationButtons: false);
        }

        private static string BackUrl()
        {
            return Url.UrlReferrer().StartsWith(Url.Server())
                ? Url.UrlReferrer()
                : Navigations.Top();
        }
    }
}