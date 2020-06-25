using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Images;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Linq;
using System.Net;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlTemplates
    {
        public static HtmlBuilder Template(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            Versions.VerTypes verType,
            BaseModel.MethodTypes methodType,
            long siteId = 0,
            long parentId = 0,
            string referenceType = null,
            string siteReferenceType = null,
            string title = null,
            string body = null,
            bool useBreadcrumb = true,
            bool useTitle = true,
            bool useSearch = true,
            bool useNavigationMenu = true,
            string script = null,
            string userScript = null,
            string userStyle = null,
            Action action = null)
        {
            return hb.Container(
                context: context,
                ss: ss,
                body: body,
                action: () => hb
                    .MainContainer(
                        context: context,
                        ss: ss,
                        view: view,
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
                    .HiddenData(
                        context: context,
                        ss: ss)
                    .VideoDialog(
                        context: context,
                        ss: ss)
                    .Styles(
                        context: context,
                        ss: ss,
                        userStyle: userStyle)
                    .Scripts(
                        context: context,
                        ss: ss,
                        script: script,
                        userScript: userScript));
        }

        private static HtmlBuilder Container(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string body,
            Action action)
        {
            if (!context.Ajax)
            {
                return hb.Html(
                    lang: context.Language,
                    action: () => hb.Head(action: () => hb
                        .Meta(httpEquiv: "X-UA-Compatible", content: "IE=edge")
                        .Meta(charset: "utf-8")
                        .Meta(name: "keywords", content: Parameters.General.HtmlHeadKeywords)
                        .Meta(name: "description", content: Description(
                            ss: ss,
                            body: body))
                        .Meta(name: "author", content: "Implem Inc.")
                        .Meta(name: "viewport", content: Parameters.General.HtmlHeadViewport)
                        .LinkedStyles(context: context)
                        .ExtendedStyles(context: context)
                        .Title(action: () => hb
                            .Text(text: HtmlTitle(
                                context: context,
                                ss: ss)))
                        .ExtendedHeader(ss: ss))
                    .Body(style: "visibility:hidden", action: action));
            }
            else
            {
                action?.Invoke();
                return hb;
            }
        }

        private static string Description(SiteSettings ss, string body)
        {
            return WebUtility.HtmlEncode(Strings.CoalesceEmpty(
                body,
                ss.Body,
                Parameters.General.HtmlHeadDescription)
                    .SplitReturn()
                    .Select(o => o.Trim())
                    .Where(o => !o.IsNullOrEmpty())
                    .Join(" "));
        }

        private static string HtmlTitle(Context context, SiteSettings ss)
        {
            switch (context.Controller)
            {
                case "items":
                case "publishes":
                    if (context.Id == 0)
                    {
                        return FormattedHtmlTitle(
                            context: context,
                            ss: ss,
                            format: context.HtmlTitleTop);
                    }
                    else if (context.Id == context.SiteId)
                    {
                        return FormattedHtmlTitle(
                            context: context,
                            ss: ss,
                            format: context.HtmlTitleSite);
                    }
                    else
                    {
                        return FormattedHtmlTitle(
                            context: context,
                            ss: ss,
                            format: context.HtmlTitleRecord);
                    }
                default:
                    return FormattedHtmlTitle(
                        context: context,
                        ss: ss,
                        format: context.HtmlTitleTop);
            }
        }

        private static string FormattedHtmlTitle(
            Context context, SiteSettings ss, string format, bool publishes = false)
        {
            return context.CanRead(ss: ss)
                ? Strings.CoalesceEmpty(
                    format?
                        .Replace("[ProductName]", Displays.ProductName(context: context))
                        .Replace("[TenantTitle]", context.TenantTitle)
                        .Replace("[SiteTitle]", context.SiteTitle)
                        .Replace("[RecordTitle]", context.RecordTitle),
                    context.TenantTitle,
                    Displays.ProductName(context: context))
                : Displays.ProductName(context: context);
        }

        private static HtmlBuilder ExtendedHeader(this HtmlBuilder hb, SiteSettings ss)
        {
            if (!ss.ExtendedHeader.IsNullOrEmpty())
            {
                hb.Raw(text: ss.ExtendedHeader);
            }
            else if (Parameters.ExtendedTags?.ContainsKey("Header") == true)
            {
                hb.Raw(text: Parameters.ExtendedTags["Header"]);
            }
            return hb;
        }

        public static HtmlBuilder MainContainer(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
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
                    context: context,
                    ss: ss,
                    siteId: siteId,
                    referenceType: referenceType,
                    errorType: errorType,
                    useNavigationMenu: useNavigationMenu,
                    useSearch: useSearch)
                .Content(
                    context: context,
                    ss: ss,
                    view: view,
                    errorType: errorType,
                    messageData: messageData,
                    siteId: siteId,
                    title: title,
                    useBreadcrumb: useBreadcrumb,
                    useTitle: useTitle,
                    action: action)
                .Div(id: "BottomMargin")
                .Footer()
                .BackUrl(
                    context: context,
                    siteId: siteId,
                    parentId: parentId,
                    referenceType: referenceType,
                    siteReferenceType: siteReferenceType));
        }

        private static HtmlBuilder Content(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            Error.Types errorType,
            string[] messageData,
            long siteId = 0,
            string title = null,
            bool useBreadcrumb = true,
            bool useTitle = true,
            Action action = null)
        {
            return hb.Div(id: "Application", action: () =>
            {
                if (!errorType.Has())
                {
                    hb.Nav(css: "both cf", action: () => hb
                        .Breadcrumb(
                            context: context,
                            ss: ss,
                            view: view,
                            _using: useBreadcrumb))
                        .Guide(
                            context: context,
                            ss: ss)
                        .Title(
                            context: context,
                            ss: ss,
                            siteId: siteId,
                            title: title,
                            useTitle: useTitle)
                        .Warnings(
                            context: context,
                            ss: ss);
                    action?.Invoke();
                    hb.Message(message: context.Message());
                }
                else
                {
                    var message = errorType.Message(
                        context: context,
                        data: messageData);
                    hb
                        .Message(message: message)
                        .MainCommands(
                            context: context,
                            ss: ss,
                            verType: Versions.VerTypes.Latest);
                }
            });
        }

        private static HtmlBuilder Title(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            long siteId,
            string title,
            bool useTitle)
        {
            return useTitle
                ? hb.Title(
                    context: context,
                    ss: ss,
                    siteId: siteId,
                    text: title)
                : hb;
        }

        public static HtmlBuilder Warnings(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Div(id: "Warnings", action: () => hb
                .SwitchUserInfo(context: context)
                .PublishWarning(
                    context: context,
                    ss: ss))
                .LockWarning(
                    context: context,
                    ss: ss);
        }

        private static HtmlBuilder SwitchUserInfo(
            this HtmlBuilder hb, Context context)
        {
            return context.SwitchUser
                ? hb.Div(id: "SwitchUserInfo", action: () => hb
                    .A(
                        href: "javascript:void(0);",
                        attributes: new HtmlAttributes()
                            .OnClick("$p.ajax('{0}','post',null,$('#SwitchUserInfo a'));".Params(
                                Locations.Get(
                                    context,
                                    "Users",
                                    "ReturnOriginalUser")))
                            .DataConfirm("ConfirmSwitchUser"),
                        action: () => hb
                            .Text(text: Displays.SwitchUserInfo(context: context))))
                : hb;
        }

        private static HtmlBuilder PublishWarning(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss)
        {
            return ss?.Publish == true && context.Authenticated
                ? hb.Div(id: "PublishWarning", action: () => hb
                    .A(
                        href: (Parameters.Service.AbsoluteUri != null
                            ? Parameters.Service.AbsoluteUri + "/"
                            : context.ApplicationPath)
                                + (context.Controller == "items"
                                    ? "publishes"
                                    : "items")
                                + "/"
                                + context.Id
                                + "/"
                                + context.Action,
                        action: () => hb
                            .Text(text: Displays.PublishWarning(context: context))))
                : hb;
        }

        private static HtmlBuilder LockWarning(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var text = !context.Publish && ss?.LockedTable() == true
                ? Displays.LockedTable(
                    context: context,
                    data: new string[]
                    {
                        ss.LockedTableUser.Name,
                        ss.LockedTableTime.DisplayValue.ToString(context.CultureInfo())
                    })
                : !context.Publish && ss?.LockedRecord() == true
                    ? Displays.LockedRecord(
                        context: context,
                        data: new string[]
                        {
                            context.Id.ToString(),
                            ss.LockedRecordUser.Name,
                            ss.LockedRecordTime.DisplayValue.ToString(context.CultureInfo())
                        })
                    : null;
            return !text.IsNullOrEmpty()
                ? hb.Div(id: "LockedWarning", action: () => hb
                    .Div(action: () => hb
                        .Text(text: text)))
                : hb;
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
            this HtmlBuilder hb, Context context, SiteSettings ss, long siteId, string text)
        {
            return !text.IsNullOrEmpty()
                ? hb
                    .Div(id: "SiteImageIconContainer", action: () => hb
                        .SiteImageIcon(context: context, ss: ss, siteId: siteId))
                    .Header(id: "HeaderTitleContainer", action: () => hb
                        .H(number: 1, id: "HeaderTitle", action: () => hb
                            .Text(text: text)))
                : hb;
        }

        public static HtmlBuilder SiteImageIcon(
            this HtmlBuilder hb, Context context, SiteSettings ss, long siteId)
        {
            return BinaryUtilities.ExistsSiteImage(
                context: context,
                ss: ss,
                referenceId: siteId,
                sizeType: ImageData.SizeTypes.Icon)
                    ? hb.Img(
                        src: Locations.Get(
                            context: context,
                            parts: new string[]
                            {
                                "Items",
                                siteId.ToString(),
                                "Binaries",
                                "SiteImageIcon",
                                BinaryUtilities.SiteImagePrefix(
                                    context: context,
                                    ss: ss,
                                    referenceId: siteId,
                                    sizeType: ImageData.SizeTypes.Icon)
                            }),
                        css: "site-image-icon")
                    : hb;
        }

        private static HtmlBuilder VideoDialog(this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return context.Authenticated
                && context.ContractSettings.Attachments() != false
                && !context.Mobile
                    ? hb
                        .Div(
                            attributes: new HtmlAttributes()
                                .Id("VideoDialog")
                                .Class("dialog")
                                .Title(Displays.Camera(context: context)),
                            action: () => hb
                                .Div(action: () => hb
                                    .Video(id: "Video"))
                                .Hidden(controlId: "VideoTarget")
                                .Div(css: "command-center", action: () => hb
                                    .Button(
                                        text: Displays.ToShoot(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.toShoot($(this));",
                                        icon: "ui-icon-video")
                                    .Button(
                                        text: Displays.Cancel(context: context),
                                        controlCss: "button-icon",
                                        onClick: "$p.closeDialog($(this));",
                                        icon: "ui-icon-cancel")))
                        .Canvas(id: "Canvas")
                    : hb;
        }

        private static HtmlBuilder HiddenData(
            this HtmlBuilder hb, Context context, SiteSettings ss = null)
        {
            return !context.Ajax
                ? hb
                    .Hidden(controlId: "ApplicationPath", value: Locations.Get(context: context))
                    .Hidden(controlId: "Language", value: context.Language)
                    .Hidden(controlId: "DeptId", value: context.DeptId.ToString())
                    .Hidden(controlId: "UserId", value: context.UserId.ToString())
                    .Hidden(controlId: "LoginId", value: context.LoginId)
                    .Hidden(controlId: "Publish", value: "1", _using: context.Publish)
                    .Hidden(controlId: "TableName", value: ss?.ReferenceType)
                    .Hidden(controlId: "Controller", value: context.Controller)
                    .Hidden(controlId: "Action", value: context.Action)
                    .Hidden(controlId: "Id", value: context.Id.ToString())
                    .Hidden(controlId: "TenantId", value: context.TenantId.ToString())
                    .Hidden(controlId: "SiteId", value: ss?.SiteId.ToString())
                    .Hidden(controlId: "JoinedSites", value: ss?.JoinedSsHash
                        ?.Select(o => new
                        {
                            SiteId = o.Key,
                            o.Value.ReferenceType,
                            o.Value.Title
                        })
                        .ToJson())
                    .HiddenSiteSettings(context: context, ss: ss)
                    .ExtendedSql(context: context)
                : hb;
        }

        private static HtmlBuilder HiddenSiteSettings(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return context.Authenticated && context.Controller == "items"
                ? hb
                    .Hidden(controlId: "ReferenceType", value: ss?.ReferenceType)
                    .Hidden(controlId: "Columns", value: ss?.ColumnsJson())
                : hb;
        }

        public static string Error(
            Context context, ErrorData errorData, string[] messageData = null)
        {
            var hb = new HtmlBuilder();
            var ss = new SiteSettings();
            return hb.Container(
                context: context,
                ss: ss,
                body: null,
                action: () => hb
                    .MainContainer(
                        context: context,
                        ss: ss,
                        view: null,
                        verType: Versions.VerTypes.Latest,
                        methodType: BaseModel.MethodTypes.NotSet,
                        messageData: messageData,
                        siteId: 0,
                        parentId: 0,
                        referenceType: null,
                        siteReferenceType: null,
                        title: null,
                        errorType: errorData.Type,
                        useBreadcrumb: false,
                        useTitle: false,
                        useNavigationMenu: true)
                    .HiddenData(context: context)
                    .Styles(
                        context: context,
                        ss: ss)
                    .Scripts(
                        context: context,
                        ss: ss))
                            .ToString();
        }
    }
}