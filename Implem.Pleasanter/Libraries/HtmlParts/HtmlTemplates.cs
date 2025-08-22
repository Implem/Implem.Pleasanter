using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Images;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlTemplates
    {
        public static HtmlBuilder Template(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
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
            BaseModel.MethodTypes methodType = BaseModel.MethodTypes.NotSet,
            ServerScriptModelRow serverScriptModelRow = null,
            Action action = null)
        {
            return hb.Container(
                context: context,
                ss: ss,
                body: body,
                methodType: methodType,
                action: () => hb
                    .Raw(HtmlHtmls.ExtendedHtmls(
                        context: context,
                        id: "HtmlBodyTop"))
                    .MainContainer(
                        context: context,
                        ss: ss,
                        view: view,
                        siteId: siteId,
                        parentId: parentId,
                        referenceType: referenceType,
                        siteReferenceType: siteReferenceType,
                        title: title,
                        useBreadcrumb: useBreadcrumb,
                        useTitle: useTitle,
                        useSearch: useSearch,
                        useNavigationMenu: useNavigationMenu,
                        serverScriptModelRow: serverScriptModelRow,
                        action: action)
                    .TemplateDialogs(
                        context: context,
                        ss: ss,
                        useNavigationMenu: useNavigationMenu)
                    .HiddenData(
                        context: context,
                        ss: ss,
                        serverScriptModelRow: serverScriptModelRow)
                    .LoaderContainer(
                        context: context,
                        ss: ss)
                    .VideoDialog(
                        context: context,
                        ss: ss)
                    .Styles(
                        context: context,
                        ss: ss,
                        userStyle: userStyle)
                    .Htmls(
                        context: context,
                        ss: ss,
                        positionType: Settings.Html.PositionTypes.BodyScriptTop,
                        methodType: methodType)
                    .Scripts(
                        context: context,
                        ss: ss,
                        script: script,
                        userScript: userScript)
                    .Htmls(
                        context: context,
                        ss: ss,
                        positionType: Settings.Html.PositionTypes.BodyScriptBottom,
                        methodType: methodType)
                    .Raw(HtmlHtmls.ExtendedHtmls(
                        context: context,
                        id: "HtmlBodyBottom")));
        }

        private static HtmlBuilder Container(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string body,
            BaseModel.MethodTypes methodType,
            Action action)
        {
            if (!context.Ajax)
            {
                return hb.Html(
                    lang: context.Language,
                    action: () => hb.Head(action: () => hb
                        .Raw(HtmlHtmls.ExtendedHtmls(
                            context: context,
                            id: "HtmlHeaderTop"))
                        .Htmls(
                            context: context,
                            ss: ss,
                            positionType: Settings.Html.PositionTypes.HeadTop,
                            methodType: methodType)
                        .Meta(httpEquiv: "X-UA-Compatible", content: "IE=edge")
                        .Meta(charset: "utf-8")
                        .Meta(name: "keywords", content: Parameters.General.HtmlHeadKeywords)
                        .Meta(name: "description", content: Description(
                            ss: ss,
                            body: body))
                        .Meta(name: "author", content: "Implem Inc.")
                        .Meta(name: "viewport", content: Parameters.General.HtmlHeadViewport)
                        .LinkedStyles(
                            context: context,
                            ss: ss)
                        .LinkedHeadLink(
                            context: context,
                            ss: ss)
                        .ExtendedStyles(context: context)
                        .Title(action: () => hb
                            .Text(text: HtmlTitle.TitleText(
                                context: context,
                                ss: ss)))
                        .ExtendedHeader(ss: ss)
                        .Htmls(
                            context: context,
                            ss: ss,
                            positionType: Settings.Html.PositionTypes.HeadBottom,
                            methodType: methodType)
                        .Raw(HtmlHtmls.ExtendedHtmls(
                            context: context,
                            id: "HtmlHeaderBottom")))
                    .Body(
                        id: context.Action == "login"
                            ? "login"
                            : string.Empty,
                        css: context.ThemeVersion1_0()
                            ? "theme-version-1_0"
                            : string.Empty,
                        style: "visibility:hidden",
                        action: action));
            }
            else
            {
                action?.Invoke();
                return hb;
            }
        }

        private static HtmlBuilder Htmls(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Settings.Html.PositionTypes positionType,
            BaseModel.MethodTypes methodType)
        {
            if (context.ContractSettings.Html == false
                || ss.HtmlsAllDisabled == true) return hb;
            hb.Raw(ss.GetHtmlBody(
                context: context,
                peredicate: o => o.All == true,
                positionType: positionType));
            var htmlBody = string.Empty;
            switch (methodType)
            {
                case BaseModel.MethodTypes.New:
                    htmlBody = ss.GetHtmlBody(
                        context: context,
                        peredicate: o => o.New == true,
                        positionType: positionType);
                    break;
                case BaseModel.MethodTypes.Edit:
                    htmlBody = ss.GetHtmlBody(
                        context: context,
                        peredicate: o => o.Edit == true,
                        positionType: positionType);
                    break;
                default:
                    htmlBody = ss.ViewModeHtmls(
                        context: context,
                        positionType: positionType);
                    break;
            }
            return hb.Raw(htmlBody);
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
            ServerScriptModelRow serverScriptModelRow = null,
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
                    useSearch: useSearch,
                    serverScriptModelRow: serverScriptModelRow)
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
                            ss: ss,
                            view: view)
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
                    hb.Message(
                        message: context.Message(),
                        messages: context.Messages);
                }
                else
                {
                    var message = errorType.Message(
                        context: context,
                        data: messageData);
                    var exceptionSiteId = context.SessionData.Get("ExceptionSiteId")?.ToLong() ?? 0;
                    hb
                        .Message(
                            message: message,
                            messages: context.Messages)
                        .MainCommands(
                            context: context,
                            ss: ss,
                            verType: Versions.VerTypes.Latest,
                            extensions: () => hb.Button(
                                controlId: "ManageTableCommand",
                                text: Displays.ManageTable(context: context),
                                controlCss: "button-icon",
                                href: Locations.ItemEdit(
                                    context: context,
                                    id: exceptionSiteId),
                                icon: "ui-icon-gear",
                                _using: exceptionSiteId > 0));
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
            if (useTitle)
            {
                if (context.ThemeVersionOver2_0())
                {
                    return hb.Div(
                        id: "TitleContainer",
                        action: () => hb.Title(
                            context: context,
                            ss: ss,
                            siteId: siteId,
                            text: title));
                }
                else
                {
                    return hb.Title(
                        context: context,
                        ss: ss,
                        siteId: siteId,
                        text: title);
                }
            }
            return hb;
        }

        public static HtmlBuilder Warnings(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Div(id: "Warnings", action: () => hb
                .SwitchUserInfo(context: context)
                .ExcessLicenseWarning(context: context)
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
            if (context.SwitchTenant)
            {
                return hb.Div(id: "SwitchUserInfo", action: () => hb
                    .A(
                        href: "#",
                        css: "void-zero",
                        attributes: new HtmlAttributes()
                            .OnClick("location.href='{0}'; ".Params(
                                Locations.Get(
                                    context,
                                    "Users",
                                    "ReturnOriginalTenant")))
                            .DataConfirm("ConfirmSwitchTenant"),
                        action: () => hb
                            .Text(text: Displays.SwitchTenantInfo(context: context))));
            }
            else
            {
                return context.SwitchUser
                    ? hb.Div(id: "SwitchUserInfo", action: () => hb
                        .A(
                            href: "#",
                            css: "void-zero",
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
        }

        private static HtmlBuilder ExcessLicenseWarning(this HtmlBuilder hb, Context context)
        {
            return UserUtilities.ExcessLicense(context: context)
                ? hb.Div(id: "ExcessLicenseWarning", action: () => hb
                    .Div(action: () => hb
                        .Text(text: Displays.ExcessLicenseWarning(
                            context: context,
                            data: Parameters.LicensedUsers().ToString()))))
                : hb;
        }

        private static HtmlBuilder PublishWarning(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss)
        {
            var wikiId = ss?.ReferenceType == "Wikis"
                ? (long?)Rds.ExecuteScalar_long(
                    context: context,
                    statements: Rds.SelectWikis(
                        column: Rds.WikisColumn().WikiId(),
                        where: Rds.WikisWhere().SiteId(ss.SiteId)))
                : null;
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
                                + (context.Controller == "items"
                                    ? wikiId ?? context.Id
                                    : context.Id).ToString()
                                + "/"
                                + (context.Id == context.SiteId && wikiId == null
                                    ? "index"
                                    : string.Empty),
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

        private static HtmlBuilder Message(
            this HtmlBuilder hb,
            Message message,
            List<Message> messages)
        {
            var data = new List<Message>();
            if (message != null)
            {
                data.Add(message);
            }
            if (messages != null)
            {
                data.AddRange(messages);
            }
            hb
                .P(id: "Message", css: "message")
                .Hidden(
                    controlId: "MessageData",
                    value: data.ToJson());
            return hb;
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
                                BinaryUtilities.SiteImageUpdatedTime(
                                    context: context,
                                    ss: ss,
                                    referenceId: siteId,
                                    sizeType: ImageData.SizeTypes.Icon)
                                    .ToString("?yyyyMMddHHmmss")
                            }),
                        css: "site-image-icon")
                    : hb;
        }

        private static HtmlBuilder VideoDialog(this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return context.Authenticated
                && context.ContractSettings.Attachments() != false
                && !context.Mobile
                && !context.Ajax
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
                                        controlCss: "button-icon button-positive",
                                        onClick: "$p.toShoot($(this));",
                                        icon: "ui-icon-video")
                                    .Button(
                                        text: Displays.Cancel(context: context),
                                        controlCss: "button-icon button-neutral",
                                        onClick: "$p.closeDialog($(this));",
                                        icon: "ui-icon-cancel")))
                        .Canvas(id: "Canvas")
                    : hb;
        }

        private static HtmlBuilder TemplateDialogs(
            this HtmlBuilder hb, Context context, SiteSettings ss, bool useNavigationMenu = true)
        {
            return !context.Ajax
                ? hb.Div(
                    id: "TemplateDialogs",
                    action: () => hb.Div(
                        id: "ChangePassword",
                        action: () => hb.Div(
                            attributes: new HtmlAttributes()
                                .Id("ChangePasswordDialog")
                                .Class("dialog")
                                .Title(Displays.ChangePassword(context: context)),
                            action: () => hb.ChangePasswordDialog(
                                context: context,
                                ss: ss,
                                content: false)),
                        _using: useNavigationMenu
                            && Parameters.Service.ShowChangePassword))
                : hb;
        }

        private static HtmlBuilder LoaderContainer(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss = null)
        {
            return !context.Ajax
                ? hb.Div(
                    id: "LoaderContainer",
                    action: () =>
                        hb.Div(css: "loader"))
                : hb;
        }

        private static HtmlBuilder HiddenData(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss = null,
            ServerScriptModelRow serverScriptModelRow = null)
        {
            return !context.Ajax
                ? hb
                    .Hidden(controlId: "ApplicationPath", value: Locations.Get(context: context))
                    .Hidden(
                        controlId: "Token",
                        value: context.Token(),
                        _using: Parameters.Security.TokenCheck)
                    .Hidden(controlId: "Language", value: context.Language)
                    .Hidden(
                        controlId: "TimeZoneOffset",
                        value: context.TimeZoneInfoOffset())
                    .Hidden(controlId: "YmdFormat", value: Displays.YmdFormat(context: context))
                    .Hidden(
                        controlId: "YmdDatePickerFormat",
                        value: Displays.YmdDatePickerFormat(context: context))
                    .Hidden(controlId: "DeptId", value: context.DeptId.ToString())
                    .Hidden(controlId: "UserId", value: context.UserId.ToString())
                    .Hidden(controlId: "LoginId", value: context.LoginId)
                    .Hidden(controlId: "GroupIds", value: (context.Groups?.Distinct()?.ToArray() ?? Array.Empty<int>()).ToJson())
                    .Hidden(controlId: "Theme", value: context.Theme())
                    .Hidden(controlId: "Publish", value: "1", _using: context.Publish)
                    .Hidden(controlId: "Responsive", value: "1", _using: context.Responsive)
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
                    .HiddenSiteSettings(
                        context: context,
                        ss: ss)
                    .HiddenServerScript(
                        context: context,
                        ss: ss,
                        serverScriptModelRow: serverScriptModelRow)
                    .ExtendedSql(context: context)
                    .Hidden(
                        controlId: "Log",
                        value: (new { Log = context.GetLog() }).ToJson())
                    .Hidden(
                        controlId: "AnchorTargetBlank",
                        value: "1",
                        _using: Parameters.General.AnchorTargetBlank)
                    .Hidden(
                        controlId: "data-validation-maxlength-type",
                        value: Parameters.Validation.MaxLengthCountType)
                    .Hidden(
                        controlId: "data-validation-maxlength-regex",
                        value: Parameters.Validation.SingleByteCharactorRegexClient)
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

        private static HtmlBuilder HiddenServerScript(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            ServerScriptModelRow serverScriptModelRow)
        {
            serverScriptModelRow?.Hidden?.ForEach(hidden => hb
                .Hidden(controlId: hidden.Key, value: hidden.Value));
            var replaceFieldColumns = serverScriptModelRow?.ReplaceFieldColumns(context: context)
                ?? new List<string>();
            hb.Hidden(
                controlId: "ReplaceFieldColumns",
                value: replaceFieldColumns.ToJson());
            return hb;
        }

        public static string Error(
            Context context,
            ErrorData errorData,
            string[] messageData = null)
        {
            var hb = new HtmlBuilder();
            var ss = new SiteSettings();
            return hb.Container(
                context: context,
                ss: ss,
                body: null,
                methodType: BaseModel.MethodTypes.NotSet,
                action: () => hb
                    .MainContainer(
                        context: context,
                        ss: ss,
                        view: null,
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