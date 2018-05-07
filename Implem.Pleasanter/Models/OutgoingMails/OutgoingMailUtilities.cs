using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class OutgoingMailUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder OutgoingMailsForm(
            this HtmlBuilder hb, string referenceType, long referenceId, int referenceVer)
        {
            var ss = SiteSettingsUtilities.GetByReference(Routes.Controller(), referenceId);
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("OutgoingMailsForm")
                    .Action(Locations.Action(Routes.Controller(), referenceId, "OutgoingMails")),
                action: () =>
                    new OutgoingMailCollection(
                        where: Rds.OutgoingMailsWhere()
                            .ReferenceType(referenceType)
                            .ReferenceId(referenceId)
                            .ReferenceVer(referenceVer, _operator: "<="),
                        orderBy: Rds.OutgoingMailsOrderBy()
                            .OutgoingMailId(SqlOrderBy.Types.desc))
                                .ForEach(outgoingMailModel => hb
                                    .OutgoingMailListItem(
                                        ss: ss, outgoingMailModel: outgoingMailModel)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder OutgoingMailListItem(
            this HtmlBuilder hb, SiteSettings ss, OutgoingMailModel outgoingMailModel)
        {
            return hb.Div(
                css: "item",
                action: () => hb
                    .H(number: 3, css: "title-header", action: () => hb
                        .Text(text: Displays.SentMail()))
                    .Div(css: "content", action: () => hb
                        .FieldText(
                            controlId: string.Empty,
                            labelText: Displays.OutgoingMails_SentTime(),
                            text: outgoingMailModel.SentTime.Value
                                .ToLocal(Displays.Get("YmdahmFormat")),
                            fieldCss: "field-auto")
                        .FieldText(
                            controlId: string.Empty,
                            labelText: Displays.OutgoingMails_From(),
                            text: outgoingMailModel.From.ToString(),
                            fieldCss: "field-auto-thin")
                        .OutgoingMailListItemDestination(
                            outgoingMailModel.To, Displays.OutgoingMails_To())
                        .OutgoingMailListItemDestination(
                            outgoingMailModel.Cc, Displays.OutgoingMails_Cc())
                        .OutgoingMailListItemDestination(
                            outgoingMailModel.Bcc, Displays.OutgoingMails_Bcc())
                        .FieldText(
                            controlId: string.Empty,
                            labelText: Displays.OutgoingMails_Title(),
                            text: outgoingMailModel.Title.Value,
                            fieldCss: "field-wide")
                        .FieldMarkUp(
                            controlId: string.Empty,
                            labelText: Displays.OutgoingMails_Body(),
                            text: outgoingMailModel.Body,
                            fieldCss: "field-wide")
                        .Div(
                            css: "command-right",
                            action: () => hb
                                .Button(
                                    text: Displays.Reply(),
                                    controlCss: "button-icon",
                                    onClick: "$p.openOutgoingMailReplyDialog($(this));",
                                    dataId: outgoingMailModel.OutgoingMailId.ToString(),
                                    icon: "ui-icon-mail-closed",
                                    action: "Reply",
                                    method: "put"),
                            _using: ss.CanSendMail())));
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
        public static string Editor(string reference, long id)
        {
            if (!Contract.Mail())
            {
                return Error.Types.Restricted.MessageJson();
            }
            if (MailAddressUtilities.Get(Sessions.UserId()) == string.Empty)
            {
                return new ResponseCollection()
                    .CloseDialog()
                    .Message(Messages.MailAddressHasNotSet())
                    .ToJson();
            }
            var ss = SiteSettingsUtilities.GetByReference(reference, id);
            var invalid = OutgoingMailValidators.OnEditing(ss);
            switch (invalid)
            {
                case Error.Types.None: break;
                default: return invalid.MessageJson();
            }
            var outgoingMailModel = new OutgoingMailModel().Get(
                where: Rds.OutgoingMailsWhere().OutgoingMailId(
                    Forms.Long("OutgoingMails_OutgoingMailId")));
            var hb = new HtmlBuilder();
            return new ResponseCollection()
                .Html("#OutgoingMailDialog", hb
                    .Div(id: "MailEditorTabsContainer", action: () => hb
                        .Ul(id: "MailEditorTabs", action: () => hb
                            .Li(action: () => hb
                                .A(
                                    href: "#FieldSetMailEditor",
                                    text: Displays.Mail()))
                            .Li(action: () => hb
                                .A(
                                    href: "#FieldSetAddressBook",
                                    text: Displays.AddressBook())))
                        .FieldSet(id: "FieldSetMailEditor", action: () => hb
                            .Form(
                                attributes: new HtmlAttributes()
                                    .Id("OutgoingMailForm")
                                    .Action(Locations.Action(
                                        reference, id, "OutgoingMails")),
                                action: () => hb
                                    .Editor(
                                        ss: ss,
                                        outgoingMailModel: outgoingMailModel)))
                        .FieldSet(id: "FieldSetAddressBook", action: () => hb
                            .Form(
                                attributes: new HtmlAttributes()
                                    .Id("OutgoingMailDestinationForm")
                                    .Action(Locations.Action(
                                        reference, id, "OutgoingMails")),
                                action: () => hb
                                    .Destinations(ss: ss)))))
                .Invoke("initOutgoingMailDialog")
                .Focus("#OutgoingMails_Body")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Editor(
            this HtmlBuilder hb,
            SiteSettings ss,
            OutgoingMailModel outgoingMailModel)
        {
            var outgoingMailSs = SiteSettingsUtilities.OutgoingMailsSiteSettings();
            var titleColumn = outgoingMailSs.GetColumn("Title");
            var bodyColumn = outgoingMailSs.GetColumn("Body");
            return hb
                .FieldBasket(
                    controlId: "OutgoingMails_To",
                    fieldCss: "field-wide",
                    controlCss: "control-basket cf",
                    labelAction: () => hb
                        .Text(text: Displays.OutgoingMails_To()))
                .FieldBasket(
                    controlId: "OutgoingMails_Cc",
                    fieldCss: "field-wide",
                    controlCss: "control-basket cf",
                    labelAction: () => hb
                        .Text(text: Displays.OutgoingMails_Cc()))
                .FieldBasket(
                    controlId: "OutgoingMails_Bcc",
                    fieldCss: "field-wide",
                    controlCss: "control-basket cf",
                    labelAction: () => hb
                        .Text(text: Displays.OutgoingMails_Bcc()))
                .FieldTextBox(
                    controlId: "OutgoingMails_Title",
                    fieldCss: "field-wide",
                    controlCss: " always-send",
                    labelText: Displays.OutgoingMails_Title(),
                    text: ReplyTitle(outgoingMailModel),
                    validateRequired: titleColumn.ValidateRequired ?? false,
                    validateMaxLength: titleColumn.ValidateMaxLength ?? 0)
                .FieldTextBox(
                    textType: HtmlTypes.TextTypes.MultiLine,
                    controlId: "OutgoingMails_Body",
                    fieldCss: "field-wide",
                    controlCss: " always-send h300",
                    labelText: Displays.OutgoingMails_Body(),
                    text: ReplyBody(outgoingMailModel),
                    validateRequired: bodyColumn.ValidateRequired ?? false,
                    validateMaxLength: bodyColumn.ValidateMaxLength ?? 0)
                .P(css: "message-dialog")
                .Div(css: "command-center", action: () => hb
                    .Button(
                        controlId: "OutgoingMails_Send",
                        controlCss: "button-icon validate",
                        text: Displays.Send(),
                        onClick: "$p.sendMail($(this));",
                        icon: "ui-icon-mail-closed",
                        action: "Send",
                        method: "post",
                        confirm: "ConfirmSendMail")
                    .Button(
                        controlId: "OutgoingMails_Cancel",
                        controlCss: "button-icon",
                        text: Displays.Cancel(),
                        onClick: "$p.closeDialog($(this));",
                        icon: "ui-icon-cancel"))
                .Hidden(controlId: "OutgoingMails_Location", value: Location())
                .Hidden(
                    controlId: "OutgoingMails_Reply",
                    value: outgoingMailModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? "1"
                        : "0")
                .Hidden(
                    controlId: "To",
                    value: MailDefault(outgoingMailModel, ss.MailToDefault, "to"))
                .Hidden(
                    controlId: "Cc",
                    value: MailDefault(outgoingMailModel, ss.MailCcDefault, "cc"))
                .Hidden(
                    controlId: "Bcc",
                    value: MailDefault(outgoingMailModel, ss.MailBccDefault, "bcc"));
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
        private static string ReplyBody(OutgoingMailModel outgoingMailModel)
        {
            return outgoingMailModel.AccessStatus == Databases.AccessStatuses.Selected
                ? Displays.OriginalMessage().Params(
                    Location(),
                    outgoingMailModel.From,
                    outgoingMailModel.SentTime.DisplayValue.ToString(
                        Displays.Get("YmdahmsFormat"), Sessions.CultureInfo()),
                    outgoingMailModel.Title.Value,
                    outgoingMailModel.Body)
                : string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string MailDefault(
            OutgoingMailModel outgoingMailModel, string mailDefault, string type)
        {
            var myAddress = new MailAddressModel(Sessions.UserId()).MailAddress;
            if (outgoingMailModel.AccessStatus == Databases.AccessStatuses.Selected)
            {
                switch (type)
                {
                    case "to":
                        var to = outgoingMailModel.To
                            .Split(';')
                            .Where(o => Libraries.Mails.Addresses.Get(o) != myAddress)
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
        private static string Location()
        {
            var location = Url.AbsoluteUri().ToLower();
            return location.Substring(0, location.IndexOf("/outgoingmails"));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder Destinations(this HtmlBuilder hb, SiteSettings ss)
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
                        controlId: "OutgoingMails_DestinationSearchRange",
                        labelText: Displays.OutgoingMails_DestinationSearchRange(),
                        optionCollection: SearchRangeOptionCollection(
                            searchRangeDefault: searchRangeDefault,
                            addressBook: addressBook),
                        controlCss: " auto-postback always-send",
                        action: "GetDestinations",
                        method: "put")
                    .FieldTextBox(
                        controlId: "OutgoingMails_DestinationSearchText",
                        labelText: Displays.OutgoingMails_DestinationSearchText(),
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
                                referenceId: ss.InheritPermission,
                                addressBook: addressBook,
                                searchRange: searchRangeDefault),
                            selectedValueCollection: new List<string>())
                        .Div(css: "command-left", action: () => hb
                            .Button(
                                controlId: "OutgoingMails_AddTo",
                                text: Displays.OutgoingMails_To(),
                                controlCss: "button-icon",
                                icon: "ui-icon-person")
                            .Button(
                                controlId: "OutgoingMails_AddCc",
                                text: Displays.OutgoingMails_Cc(),
                                controlCss: "button-icon",
                                icon: "ui-icon-person")
                            .Button(
                                controlId: "OutgoingMails_AddBcc",
                                text: Displays.OutgoingMails_Bcc(),
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
            string searchRangeDefault, Dictionary<string, ControlData> addressBook)
        {
            switch (searchRangeDefault)
            {
                case "DefaultAddressBook":
                    return new Dictionary<string, ControlData>
                    {
                        { "DefaultAddressBook", new ControlData(Displays.DefaultAddressBook()) },
                        { "SiteUser", new ControlData(Displays.SiteUser()) },
                        { "All", new ControlData(Displays.All()) }
                    };
                case "SiteUser":
                    return new Dictionary<string, ControlData>
                    {
                        { "SiteUser", new ControlData(Displays.SiteUser()) },
                        { "All", new ControlData(Displays.All()) }
                    };
                default:
                    return new Dictionary<string, ControlData>
                    {
                        { "All", new ControlData(Displays.All()) }
                    };
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Dictionary<string, ControlData> Destinations(
            long referenceId,
            Dictionary<string, ControlData> addressBook,
            string searchRange,
            string searchText = "")
        {
            var joinDepts = new SqlJoin(
                "[Depts]",
                SqlJoin.JoinTypes.LeftOuter,
                "[Users].[DeptId]=[Depts].[DeptId]");
            var joinMailAddresses = new SqlJoin(
                "[MailAddresses]",
                SqlJoin.JoinTypes.Inner,
                "[Users].[UserId]=[MailAddresses].[OwnerId]");
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
                    var joinPermissions = new SqlJoin(
                        "[Permissions]",
                        SqlJoin.JoinTypes.Inner,
                        "([Users].[UserId]=[Permissions].[UserId] and [Permissions].[UserId] <> 0) or " +
                        "([Users].[DeptId]=[Permissions].[DeptId] and [Permissions].[DeptId] <> 0)");
                    return DestinationCollection(
                        Sqls.SqlJoinCollection(joinDepts, joinMailAddresses, joinPermissions),
                        Rds.UsersWhere()
                            .MailAddresses_OwnerType("Users")
                            .Permissions_ReferenceId(referenceId)
                            .SearchText(searchText)
                            .Users_TenantId(Sessions.TenantId()));
                case "All":
                default:
                    return !searchText.IsNullOrEmpty()
                        ? DestinationCollection(
                            Sqls.SqlJoinCollection(joinDepts, joinMailAddresses),
                            Rds.UsersWhere()
                                .MailAddresses_OwnerType("Users")
                                .SearchText(searchText)
                                .Users_TenantId(Sessions.TenantId()))
                        : new Dictionary<string, ControlData>();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SqlWhereCollection SearchText(
            this SqlWhereCollection self, string searchText)
        {
            return self
                .SqlWhereLike(
                    name: "SearchText",
                    searchText: searchText,
                    clauseCollection: new List<string>()
                    {
                        Rds.Users_LoginId_WhereLike(),
                        Rds.Users_Name_WhereLike(),
                        Rds.Users_UserCode_WhereLike(),
                        Rds.Users_Body_WhereLike(),
                        Rds.Depts_DeptCode_WhereLike(),
                        Rds.Depts_DeptName_WhereLike(),
                        Rds.Depts_Body_WhereLike(),
                        Rds.MailAddresses_MailAddress_WhereLike("MailAddresses")
                    })
                .Users_Disabled(0);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static Dictionary<string, ControlData> DestinationCollection(
            SqlJoinCollection join, SqlWhereCollection where)
        {
            return Rds.ExecuteTable(
                transactional: false,
                statements: Rds.SelectUsers(
                    column: Rds.UsersColumn()
                        .Name()
                        .MailAddresses_MailAddress(),
                    join: join,
                    where: where,
                    distinct: true)).AsEnumerable()
                        .Select((o, i) => new {
                            Name = "\"" + o["Name"].ToString() +
                                "\" <" + o["MailAddress"].ToString() + ">",
                            Index = i
                        })
                        .ToDictionary(
                            o => o.Index.ToString(),
                            o => new ControlData(o.Name));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static System.Net.Mail.MailAddress From()
        {
            return new System.Net.Mail.MailAddress(MailAddressUtilities.Get(
                Sessions.UserId(), withFullName: true));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Send(string reference, long id)
        {
            if (!Contract.Mail())
            {
                return Error.Types.Restricted.MessageJson();
            }
            var ss = SiteSettingsUtilities.GetByReference(reference, id);
            var outgoingMailModel = new OutgoingMailModel(reference, id);
            var invalidMailAddress = string.Empty;
            var invalid = OutgoingMailValidators.OnSending(
                ss, outgoingMailModel, out invalidMailAddress);
            switch (invalid)
            {
                case Error.Types.None:
                    break;
                case Error.Types.BadMailAddress:
                case Error.Types.ExternalMailAddress:
                    return invalid.MessageJson(invalidMailAddress);
                default:
                    return invalid.MessageJson();
            }
            var error = outgoingMailModel.Send();
            return error.Has()
                ? error.MessageJson()
                : new OutgoingMailsResponseCollection(outgoingMailModel)
                    .CloseDialog()
                    .ClearFormData()
                    .Html("#OutgoingMailDialog", string.Empty)
                    .Val("#OutgoingMails_Title", string.Empty)
                    .Val("#OutgoingMails_Body", string.Empty)
                    .Prepend(
                        "#OutgoingMailsForm",
                        new HtmlBuilder().OutgoingMailListItem(
                            ss: ss, outgoingMailModel: outgoingMailModel))
                    .Message(Messages.MailTransmissionCompletion())
                    .ToJson();
        }
    }
}
