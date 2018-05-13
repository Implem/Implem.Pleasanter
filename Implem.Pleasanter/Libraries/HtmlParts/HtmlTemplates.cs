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
using System.Linq;
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
                .HiddenData(ss: ss)
                .VideoDialog(ss: ss)
                .Styles(ss: ss, userStyle: userStyle)
                .Scripts(ss: ss, script: script, userScript: userScript);
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
            string[] messageData = null,
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
                    messageData: messageData,
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
            string[] messageData,
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
                    hb.Message(message: Sessions.Message());
                }
                else
                {
                    var message = errorType.Message(messageData);
                    hb
                        .Message(message: message)
                        .MainCommands(
                            siteId: siteId,
                            ss: ss,
                            verType: Versions.VerTypes.Latest);
                }
            });
        }

        private static HtmlBuilder Message(this HtmlBuilder hb, Message message)
        {
            return hb
                .P(id: "Message", css: "message")
                .Hidden(
                    controlId: "MessageData",
                    value: message?.ToJson(),
                    _using: message != null);
        }

        private static HtmlBuilder Title(
            this HtmlBuilder hb, SiteSettings ss, long siteId, string text)
        {
            return !text.IsNullOrEmpty()
                ? hb
                    .Div(id: "SiteImageIconContainer", action: () => hb
                        .SiteImageIcon(ss: ss, siteId: siteId))
                    .Header(id: "HeaderTitleContainer", action: () => hb
                        .H(number: 1, id: "HeaderTitle", action: () => hb
                            .Text(text: text)))
                : hb;
        }

        public static HtmlBuilder SiteImageIcon(
            this HtmlBuilder hb, SiteSettings ss, long siteId)
        {
            return BinaryUtilities.ExistsSiteImage(ss, siteId, ImageData.SizeTypes.Icon)
                ? hb.Img(
                    src: Locations.Get(
                        "Items",
                        siteId.ToString(),
                        "Binaries",
                        "SiteImageIcon",
                        BinaryUtilities.SiteImagePrefix(ss, siteId, ImageData.SizeTypes.Icon)),
                    css: "site-image-icon")
                : hb;
        }

        private static HtmlBuilder VideoDialog(this HtmlBuilder hb, SiteSettings ss)
        {
            return Contract.Attachments() && !ss.Mobile
                ? hb
                    .Div(
                        attributes: new HtmlAttributes()
                            .Id("VideoDialog")
                            .Class("dialog")
                            .Title(Displays.Camera()),
                        action: () => hb
                            .Div(action: () => hb
                                .Video(id: "Video"))
                            .Hidden(controlId: "VideoTarget")
                            .Div(css: "command-center", action: () => hb
                                .Button(
                                    text: Displays.ToShoot(),
                                    controlCss: "button-icon",
                                    onClick: "$p.toShoot($(this));",
                                    icon: "ui-icon-video")
                                .Button(
                                    text: Displays.Cancel(),
                                    controlCss: "button-icon",
                                    onClick: "$p.closeDialog($(this));",
                                    icon: "ui-icon-cancel")))
                    .Canvas(id: "Canvas")
                : hb;
        }

        private static HtmlBuilder HiddenData(this HtmlBuilder hb, SiteSettings ss = null)
        {
            return !Request.IsAjax()
                ? hb
                    .Hidden(controlId: "ApplicationPath", value: Locations.Get())
                    .Hidden(controlId: "Language", value: Sessions.Language())
                    .Hidden(controlId: "DeptId", value: Sessions.DeptId().ToString())
                    .Hidden(controlId: "UserId", value: Sessions.UserId().ToString())
                : hb;
        }

        public static string Error(Error.Types errorType, string[] messageData = null)
        {
            var hb = new HtmlBuilder();
            return hb
                .MainContainer(
                    ss: new SiteSettings(),
                    verType: Versions.VerTypes.Latest,
                    methodType: BaseModel.MethodTypes.NotSet,
                    messageData: messageData,
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
                .Styles()
                .Scripts()
                .ToString();
        }
    }
}