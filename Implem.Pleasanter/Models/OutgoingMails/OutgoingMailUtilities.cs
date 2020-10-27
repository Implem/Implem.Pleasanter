using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
namespace Implem.Pleasanter.Models
{
    public static class OutgoingMailUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder OutgoingMailsForm(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string referenceType,
            long referenceId,
            int referenceVer)
        {
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("OutgoingMailsForm")
                    .Action(Locations.Action(
                        context: context,
                        table: context.Controller,
                        id: referenceId,
                        controller: "OutgoingMails")),
                action: () =>
                    new OutgoingMailCollection(
                        context: context,
                        where: Rds.OutgoingMailsWhere()
                            .ReferenceType(referenceType)
                            .ReferenceId(referenceId)
                            .ReferenceVer(referenceVer, _operator: "<="),
                        orderBy: Rds.OutgoingMailsOrderBy()
                            .OutgoingMailId(SqlOrderBy.Types.desc))
                                .ForEach(outgoingMailModel => hb
                                    .OutgoingMailListItem(
                                        context: context,
                                        ss: ss,
                                        outgoingMailModel: outgoingMailModel)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder OutgoingMailListItem(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            OutgoingMailModel outgoingMailModel)
        {
            return hb.Div(
                css: "item",
                action: () => hb
                    .H(number: 3, css: "title-header", action: () => hb
                        .Text(text: Displays.SentMail(context: context)))
                    .Div(css: "content", action: () => hb
                        .FieldText(
                            controlId: string.Empty,
                            labelText: Displays.OutgoingMails_SentTime(context: context),
                            text: outgoingMailModel.SentTime.Value
                                .ToLocal(
                                    context: context,
                                    format: Displays.Get(
                                        context: context,
                                        id: "YmdahmFormat")),
                            fieldCss: "field-auto")
                        .FieldText(
                            controlId: string.Empty,
                            labelText: Displays.OutgoingMails_From(context: context),
                            text: outgoingMailModel.From.ToString(),
                            fieldCss: "field-auto-thin")
                        .OutgoingMailListItemDestination(
                            outgoingMailModel.To, Displays.OutgoingMails_To(context: context))
                        .OutgoingMailListItemDestination(
                            outgoingMailModel.Cc, Displays.OutgoingMails_Cc(context: context))
                        .OutgoingMailListItemDestination(
                            outgoingMailModel.Bcc, Displays.OutgoingMails_Bcc(context: context))
                        .FieldText(
                            controlId: string.Empty,
                            labelText: Displays.OutgoingMails_Title(context: context),
                            text: outgoingMailModel.Title.Value,
                            fieldCss: "field-wide")
                        .FieldMarkUp(
                            controlId: string.Empty,
                            labelText: Displays.OutgoingMails_Body(context: context),
                            text: outgoingMailModel.Body,
                            fieldCss: "field-wide")
                        .Div(
                            css: "command-right",
                            action: () => hb
                                .Button(
                                    text: Displays.Reply(context: context),
                                    controlCss: "button-icon",
                                    onClick: "$p.openOutgoingMailReplyDialog($(this));",
                                    dataId: outgoingMailModel.OutgoingMailId.ToString(),
                                    icon: "ui-icon-mail-closed",
                                    action: "Reply",
                                    method: "put"),
                            _using: context.CanSendMail(ss: ss))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder OutgoingMailListItemDestination(
            this HtmlBuilder hb, string destinations, string labelText)
        {
            return destinations != string.Empty
                ? hb.FieldText(
                    controlId: string.Empty,
                    labelText: labelText,
                    text: destinations,
                    fieldCss: "field-wide")
                : hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder OutgoingMailDialog(this HtmlBuilder hb)
        {
            return hb.Div(attributes: new HtmlAttributes()
                .Id("OutgoingMailDialog")
                .Class("dialog"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Editor(Context context, string reference, long id)
        {
            var ss = SiteSettingsUtilities.GetByReference(
                context: context,
                reference: reference,
                referenceId: id);
            if (context.ContractSettings.Mail == false)
            {
                return Error.Types.Restricted.MessageJson(context: context);
            }
            if (MailAddressUtilities.Get(
                context: context,
                userId: context.UserId) == string.Empty)
            {
                return new ResponseCollection()
                    .CloseDialog()
                    .Message(Messages.MailAddressHasNotSet(context: context))
                    .ToJson();
            }
            var invalid = OutgoingMailValidators.OnEditing(
                context: context,
                ss: ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson(context: context);
            }
            var outgoingMailModel = new OutgoingMailModel().Get(
                context: context,
                where: Rds.OutgoingMailsWhere().OutgoingMailId(
                    context.Forms.Long("OutgoingMails_OutgoingMailId")));
            var hb = new HtmlBuilder();
            return new ResponseCollection()
                .Html("#OutgoingMailDialog", hb
                    .Div(id: "MailEditorTabsContainer", action: () => hb
                        .Ul(id: "MailEditorTabs", action: () => hb
                            .Li(action: () => hb
                                .A(
                                    href: "#FieldSetMailEditor",
                                    text: Displays.Mail(context: context)))
                            .Li(action: () => hb
                                .A(
                                    href: "#FieldSetAddressBook",
                                    text: Displays.AddressBook(context: context))))
                        .FieldSet(id: "FieldSetMailEditor", action: () => hb
                            .Form(
                                attributes: new HtmlAttributes()
                                    .Id("OutgoingMailForm")
                                    .Action(Locations.Action(
                                        context: context,
                                        table: reference,
                                        id: id,
                                        controller: "OutgoingMails")),
                                action: () => hb
                                    .Editor(
                                        context: context,
                                        ss: ss,
                                        outgoingMailModel: outgoingMailModel)))
                        .FieldSet(id: "FieldSetAddressBook", action: () => hb
                            .Form(
                                attributes: new HtmlAttributes()
                                    .Id("OutgoingMailDestinationForm")
                                    .Action(Locations.Action(
                                        context: context,
                                        table: reference,
                                        id: id,
                                        controller: "OutgoingMails")),
                                action: () => hb
                                    .Destinations(context: context, ss: ss)))))
                .Invoke("initOutgoingMailDialog")
                .Focus("#OutgoingMails_Body")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            OutgoingMailModel outgoingMailModel)
        {
            var outgoingMailSs = SiteSettingsUtilities
                .OutgoingMailsSiteSettings(context: context);
            var titleColumn = outgoingMailSs.GetColumn(
                context: context,
                columnName: "Title");
            var bodyColumn = outgoingMailSs.GetColumn(
                context: context,
                columnName: "Body");
            return hb
                .FieldBasket(
                    controlId: "OutgoingMails_To",
                    fieldCss: "field-wide",
                    controlCss: "control-basket cf",
                    labelAction: () => hb
                        .Text(text: Displays.OutgoingMails_To(context: context)))
                .FieldBasket(
                    controlId: "OutgoingMails_Cc",
                    fieldCss: "field-wide",
                    controlCss: "control-basket cf",
                    labelAction: () => hb
                        .Text(text: Displays.OutgoingMails_Cc(context: context)))
                .FieldBasket(
                    controlId: "OutgoingMails_Bcc",
                    fieldCss: "field-wide",
                    controlCss: "control-basket cf",
                    labelAction: () => hb
                        .Text(text: Displays.OutgoingMails_Bcc(context: context)))
                .FieldTextBox(
                    controlId: "OutgoingMails_Title",
                    fieldCss: "field-wide",
                    controlCss: " always-send",
                    labelText: Displays.OutgoingMails_Title(context: context),
                    text: ReplyTitle(outgoingMailModel),
                    validateRequired: titleColumn.ValidateRequired ?? false,
                    validateMaxLength: titleColumn.ValidateMaxLength ?? 0)
                .FieldTextBox(
                    textType: HtmlTypes.TextTypes.MultiLine,
                    controlId: "OutgoingMails_Body",
                    fieldCss: "field-wide",
                    controlCss: " always-send h300",
                    labelText: Displays.OutgoingMails_Body(context: context),
                    text: ReplyBody(
                        context: context,
                        outgoingMailModel: outgoingMailModel),
                    validateRequired: bodyColumn.ValidateRequired ?? false,
                    validateMaxLength: bodyColumn.ValidateMaxLength ?? 0)
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "OutgoingMails_Send",
                        controlCss: "button-icon validate",
                        text: Displays.Send(context: context),
                        onClick: "$p.sendMail($(this));",
                        icon: "ui-icon-mail-closed",
                        action: "Send",
                        method: "post",
                        confirm: "ConfirmSendMail")
                    .Button(
                        controlId: "OutgoingMails_Cancel",
                        controlCss: "button-icon",
                        text: Displays.Cancel(context: context),
                        onClick: "$p.closeDialog($(this));",
                        icon: "ui-icon-cancel"))
                .Hidden(
                    controlId: "OutgoingMails_Location",
                    value: Locations.OutGoingMailAbsoluteUri(context: context))
                .Hidden(
                    controlId: "OutgoingMails_Reply",
                    value: outgoingMailModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? "1"
                        : "0")
                .Hidden(
                    controlId: "To",
                    value: MailDefault(
                        context: context,
                        ss: ss,
                        outgoingMailModel: outgoingMailModel,
                        mailDefault: ss.MailToDefault,
                        type: "to"))
                .Hidden(
                    controlId: "Cc",
                    value: MailDefault(
                        context: context,
                        ss: ss,
                        outgoingMailModel: outgoingMailModel,
                        mailDefault: ss.MailCcDefault,
                        type: "cc"))
                .Hidden(
                    controlId: "Bcc",
                    value: MailDefault(
                        context: context,
                        ss: ss,
                        outgoingMailModel: outgoingMailModel,
                        mailDefault: ss.MailBccDefault,
                        type: "bcc"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string ReplyTitle(OutgoingMailModel outgoingMailModel)
        {
            var title = outgoingMailModel.Title.Value;
            return outgoingMailModel.AccessStatus == Databases.AccessStatuses.Selected
                ? title.StartsWith("Re: ") ? title : "Re: " + title
                : string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string ReplyBody(Context context, OutgoingMailModel outgoingMailModel)
        {
            return outgoingMailModel.AccessStatus == Databases.AccessStatuses.Selected
                ? Displays.OriginalMessage(context: context).Params(
                    Locations.OutGoingMailAbsoluteUri(context: context),
                    outgoingMailModel.From,
                    outgoingMailModel.SentTime.DisplayValue.ToString(
                        Displays.Get(
                            context: context,
                            id: "YmdahmsFormat"),
                        context.CultureInfo()),
                    outgoingMailModel.Title.Value,
                    outgoingMailModel.Body)
                : string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string MailDefault(
            Context context,
            SiteSettings ss,
            OutgoingMailModel outgoingMailModel,
            string mailDefault,
            string type)
        {
            var myAddress = new MailAddressModel(
                context: context,
                userId: context.UserId).MailAddress;
            if (outgoingMailModel.AccessStatus == Databases.AccessStatuses.Selected)
            {
                switch (type)
                {
                    case "to":
                        var to = outgoingMailModel.To
                            .Split(';')
                            .Where(o => Libraries.Mails.Addresses.GetBody(o) != myAddress)
                            .Where(o => o.Trim() != string.Empty)
                            .Join(";");
                        return to.Trim() != string.Empty
                            ? outgoingMailModel.From.ToString() + ";" + to
                            : outgoingMailModel.From.ToString();
                    case "cc":
                        return outgoingMailModel.Cc;
                    case "bcc":
                        return outgoingMailModel.Bcc;
                }
            }
            return mailDefault;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Destinations(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var addressBook = AddressBook(ss);
            var searchRangeDefault = ss.SiteId != 0
                ? addressBook.Count > 0
                    ? "DefaultAddressBook"
                    : "SiteUser"
                : "All";
            return hb
                .Div(css: "container-left", action: () => hb
                    .FieldDropDown(
                        context: context,
                        controlId: "OutgoingMails_DestinationSearchRange",
                        labelText: Displays.OutgoingMails_DestinationSearchRange(context: context),
                        optionCollection: SearchRangeOptionCollection(
                            context: context,
                            searchRangeDefault: searchRangeDefault,
                            addressBook: addressBook),
                        controlCss: " auto-postback always-send",
                        action: "GetDestinations",
                        method: "put")
                    .FieldTextBox(
                        controlId: "OutgoingMails_DestinationSearchText",
                        labelText: Displays.OutgoingMails_DestinationSearchText(context: context),
                        controlCss: " auto-postback",
                        action: "GetDestinations",
                        method: "put"))
                .Div(css: "container-right", action: () => hb
                    .Div(action: () => hb
                        .FieldSelectable(
                            controlId: "OutgoingMails_MailAddresses",
                            fieldCss: "field-vertical both",
                            controlContainerCss: "container-selectable",
                            controlWrapperCss: " h500",
                            listItemCollection: Destinations(
                                context: context,
                                ss: ss,
                                referenceId: ss.InheritPermission,
                                addressBook: addressBook,
                                searchRange: searchRangeDefault),
                            selectedValueCollection: new List<string>())
                        .Div(css: "command-left", action: () => hb
                            .Button(
                                controlId: "OutgoingMails_AddTo",
                                text: Displays.OutgoingMails_To(context: context),
                                controlCss: "button-icon",
                                icon: "ui-icon-person")
                            .Button(
                                controlId: "OutgoingMails_AddCc",
                                text: Displays.OutgoingMails_Cc(context: context),
                                controlCss: "button-icon",
                                icon: "ui-icon-person")
                            .Button(
                                controlId: "OutgoingMails_AddBcc",
                                text: Displays.OutgoingMails_Bcc(context: context),
                                controlCss: "button-icon",
                                icon: "ui-icon-person"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Dictionary<string, ControlData> AddressBook(SiteSettings ss)
        {
            return ss.AddressBook
                .SplitReturn()
                .Select(o => o.Trim())
                .Where(o => o != string.Empty)
                .ToDictionary(o => o, o => new ControlData(o));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, ControlData> SearchRangeOptionCollection(
            Context context,
            string searchRangeDefault,
            Dictionary<string, ControlData> addressBook)
        {
            switch (searchRangeDefault)
            {
                case "DefaultAddressBook":
                    return new Dictionary<string, ControlData>
                    {
                        {
                            "DefaultAddressBook",
                            new ControlData(Displays.DefaultAddressBook(context: context))
                        },
                        {
                            "SiteUser",
                            new ControlData(Displays.SiteUser(context: context))
                        },
                        {
                            "All",
                            new ControlData(Displays.All(context: context))
                        }
                    };
                case "SiteUser":
                    return new Dictionary<string, ControlData>
                    {
                        {
                            "SiteUser",
                            new ControlData(Displays.SiteUser(context: context))
                        },
                        {
                            "All",
                            new ControlData(Displays.All(context: context))
                        }
                    };
                default:
                    return new Dictionary<string, ControlData>
                    {
                        {
                            "All",
                            new ControlData(Displays.All(context: context))
                        }
                    };
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Dictionary<string, ControlData> Destinations(
            Context context,
            SiteSettings ss,
            long referenceId,
            Dictionary<string, ControlData> addressBook,
            string searchRange,
            string searchText = "")
        {
            var joinDepts = new SqlJoin(
                "\"Depts\"",
                SqlJoin.JoinTypes.LeftOuter,
                "\"Users\".\"DeptId\"=\"Depts\".\"DeptId\"");
            var joinMailAddresses = new SqlJoin(
                "\"MailAddresses\"",
                SqlJoin.JoinTypes.Inner,
                "\"Users\".\"UserId\"=\"MailAddresses\".\"OwnerId\"");
            switch (searchRange)
            {
                case "DefaultAddressBook":
                    return searchText == string.Empty
                        ? addressBook
                        : addressBook
                            .Where(o => o.Value.Text.IndexOf(searchText,
                                System.Globalization.CompareOptions.IgnoreCase |
                                System.Globalization.CompareOptions.IgnoreKanaType |
                                System.Globalization.CompareOptions.IgnoreWidth) != -1)
                            .ToDictionary(o => o.Key, o => new ControlData(o.Value.Text));
                case "SiteUser":
                    return DestinationCollection(
                        context: context,
                        join: Sqls.SqlJoinCollection(
                            joinDepts,
                            joinMailAddresses),
                        where: Rds.UsersWhere()
                            .SiteUserWhere(siteId: referenceId)
                            .MailAddresses_OwnerType("Users")
                            .SearchText(
                                context: context,
                                searchText: searchText)
                            .Users_TenantId(context.TenantId));
                case "All":
                default:
                    return !searchText.IsNullOrEmpty()
                        ? DestinationCollection(
                            context: context,
                            join: Sqls.SqlJoinCollection(
                                joinDepts,
                                joinMailAddresses),
                            where: Rds.UsersWhere()
                                .MailAddresses_OwnerType("Users")
                                .SearchText(
                                    context: context,
                                    searchText: searchText)
                                .Users_TenantId(context.TenantId))
                        : new Dictionary<string, ControlData>();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SqlWhereCollection SearchText(
            this SqlWhereCollection self, Context context, string searchText)
        {
            return self
                .SqlWhereLike(
                    tableName: "Users",
                    name: "SearchText",
                    searchText: searchText,
                    clauseCollection: new List<string>()
                    {
                        Rds.Users_LoginId_WhereLike(factory: context),
                        Rds.Users_Name_WhereLike(factory: context),
                        Rds.Users_UserCode_WhereLike(factory: context),
                        Rds.Users_Body_WhereLike(factory: context),
                        Rds.Depts_DeptCode_WhereLike(factory: context),
                        Rds.Depts_DeptName_WhereLike(factory: context),
                        Rds.Depts_Body_WhereLike(factory: context),
                        Rds.MailAddresses_MailAddress_WhereLike(
                            factory: context,
                            tableName:"MailAddresses")
                    })
                .Users_Disabled(false);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, ControlData> DestinationCollection(
            Context context, SqlJoinCollection join, SqlWhereCollection where)
        {
            return Repository.ExecuteTable(
                context: context,
                transactional: false,
                statements: Rds.SelectUsers(
                    column: Rds.UsersColumn()
                        .Name()
                        .MailAddresses_MailAddress(),
                    join: join,
                    where: where,
                    distinct: true)).AsEnumerable()
                        .Select((o, i) => new {
                            Name = $"\"{o.String("Name")}\" <{o.String("MailAddress")}>",
                            Index = i
                        })
                        .ToDictionary(
                            o => o.Index.ToString(),
                            o => new ControlData(o.Name));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Net.Mail.MailAddress From(Context context, int userId)
        {
            return new System.Net.Mail.MailAddress(
                MailAddressUtilities.Get(
                    context: context,
                    userId: userId,
                    withFullName: true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Send(Context context, string reference, long id)
        {
            var ss = SiteSettingsUtilities.GetByReference(
                context: context,
                reference: reference,
                referenceId: id);
            if (context.ContractSettings.Mail == false)
            {
                return Error.Types.Restricted.MessageJson(context: context);
            }
            var outgoingMailModel = new OutgoingMailModel(
                context: context,
                reference: reference,
                referenceId: id);
            var invalid = OutgoingMailValidators.OnSending(
                context: context,
                ss: ss,
                outgoingMailModel: outgoingMailModel);
            switch (invalid.Type)
            {
                case Error.Types.None:
                    break;
                case Error.Types.BadMailAddress:
                case Error.Types.ExternalMailAddress:
                    return invalid.MessageJson(context: context);
                default:
                    return invalid.MessageJson(context: context);
            }
            var errorData = outgoingMailModel.Send(
                context: context,
                ss: ss);
            return errorData.Type.Has()
                ? errorData.MessageJson(context: context)
                : new OutgoingMailsResponseCollection(outgoingMailModel)
                    .CloseDialog()
                    .ClearFormData()
                    .Html("#OutgoingMailDialog", string.Empty)
                    .Val("#OutgoingMails_Title", string.Empty)
                    .Val("#OutgoingMails_Body", string.Empty)
                    .Prepend(
                        "#OutgoingMailsForm",
                        new HtmlBuilder().OutgoingMailListItem(
                            context: context,
                            ss: ss,
                            outgoingMailModel: outgoingMailModel))
                    .Message(Messages.MailTransmissionCompletion(context: context))
                    .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Web.Mvc.ContentResult SendByApi(Context context, string reference, long id)
        {
            if (!Mime.ValidateOnApi(contentType: context.ContentType))
            {
                return ApiResults.BadRequest(context: context);
            }
            var itemModel = new ItemModel(
                context: context,
                referenceId: id);
            var siteModel = new SiteModel(
                context: context,
                siteId: itemModel.SiteId);
            var ss = SiteSettingsUtilities.Get(
                context: context,
                siteModel: siteModel,
                referenceId: itemModel.ReferenceId);
            var outgoingMailModel = new OutgoingMailModel(
                context: context,
                reference: reference,
                referenceId: id);
            var data = context.RequestDataString.Deserialize<OutgoingMailApiModel>();
            if (data == null)
            {
                return ApiResults.Get(ApiResponses.BadRequest(context: context));
            }
            if (!siteModel.WithinApiLimits())
            {
                return ApiResults.Get(ApiResponses.OverLimitApi(
                    context: context,
                    siteId: itemModel.SiteId,
                    limitPerSite: Parameters.Api.LimitPerSite));
            }
            if (data.From != null) outgoingMailModel.From = new System.Net.Mail.MailAddress(data.From);
            if (data.To != null) outgoingMailModel.To = data.To;
            if (data.Cc != null) outgoingMailModel.Cc = data.Cc;
            if (data.Bcc != null) outgoingMailModel.Bcc = data.Bcc;
            if (data.Title != null) outgoingMailModel.Title = new Title(data.Title);
            if (data.Body != null) outgoingMailModel.Body = data.Body;
            var invalid = OutgoingMailValidators.OnSending(
                context: context,
                ss: ss,
                outgoingMailModel: outgoingMailModel);
            switch (invalid.Type)
            {
                case Error.Types.None: break;
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: invalid);
            }
            var errorData = outgoingMailModel.Send(
                context: context,
                ss: ss);
            switch (errorData.Type)
            {
                case Error.Types.None:
                    SiteUtilities.UpdateApiCount(context: context, ss: ss);
                    return ApiResults.Success(
                        id: id,
                        limitPerDate: Parameters.Api.LimitPerSite,
                        limitRemaining: Parameters.Api.LimitPerSite - ss.ApiCount,
                        message: Displays.MailTransmissionCompletion(
                            context: context,
                            data: outgoingMailModel.Title.DisplayValue));
                default:
                    return ApiResults.Error(
                        context: context,
                        errorData: errorData);
            }
        }
    }
}
