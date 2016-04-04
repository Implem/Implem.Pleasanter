using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Utilities;
using Implem.Pleasanter.Models;
using System;
namespace Implem.Pleasanter.Libraries.Views
{
    public static class HtmlTemplates
    {
        public static HtmlBuilder Template(
            this HtmlBuilder hb,
            long siteId,
            string modelName,
            string title,
            Permissions.Types permissionType,
            Versions.VerTypes verType,
            string backUrl,
            BaseModel.MethodTypes methodType,
            bool allowAccess,
            bool useBreadCrumbs = true,
            bool useTitle = true,
            bool useSearch = true,
            bool useNavigationButtons = true,
            string script = "",
            Action action = null)
        {
            return hb
                .PageHeader()
                .Article(id: "application", action: () => 
                {
                    if (allowAccess)
                    {
                        hb.Nav(css: "both cf", action: () => hb
                            .BreadCrumbs(
                                siteId: siteId,
                                permissionType: permissionType,
                                _using: useBreadCrumbs)
                            .NavigationFunctions(
                                siteId: siteId,
                                permissionType: permissionType,
                                useSearch: useSearch,
                                useNavigationButtons: useNavigationButtons));
                        if (useTitle)
                        {
                            hb.Title(permissionType: permissionType, siteId: siteId, text: title);
                        }
                        action();
                        if (verType != Versions.VerTypes.Latest &&
                            verType != Versions.VerTypes.Unknown)
                        {
                            hb.P(id: "Message", css: "message", action: () => hb
                                .Span(css: "alert-information", action: () => hb
                                    .Displays_ReadOnlyBecausePreviousVer()));
                        }
                        else
                        {
                            hb.P(id: "Message", css: "message", action: () => hb
                                .Raw(text: string.Empty));
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
                                verType: Utilities.Versions.VerTypes.Latest,
                                backUrl: BackUrl());
                    }
                })
                .Div(css: "margin-bottom both")
                .Footer(css: "footer", action: () => hb
                    .P(action: () => hb
                        .A(
                            attributes: Html.Attributes().Href(Parameters.HtmlCopyrightUrl),
                            action: () => hb
                                .Raw(Parameters.HtmlCopyright.Params(DateTime.Now.Year)))))
                .Hidden(controlId: "ApplicationPath", value: Navigations.Get())
                .Scripts(
                    methodType: methodType,
                    script: script,
                    modelName: modelName,
                    allowAccess: allowAccess);
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

        public static HtmlBuilder NotFound()
        {
            return Html.Builder()
                .Template(
                    siteId: 0,
                    modelName: string.Empty,
                    title: string.Empty,
                    permissionType: Permissions.Types.NotSet,
                    verType: Versions.VerTypes.Unknown,
                    backUrl: BackUrl(),
                    methodType: BaseModel.MethodTypes.NotSet,
                    allowAccess: false,
                    useBreadCrumbs: false,
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