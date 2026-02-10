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
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    public static class PasskeyUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder PasskeyDialog(this HtmlBuilder hb, Context context, bool _using = true)
        {
            return hb.Div(
                _using: _using,
                attributes: new HtmlAttributes()
                    .Id("PasskeyDialog")
                    .Class("dialog")
                    .Title(Displays.Passkey(context: context)),
                action: () => hb
                    .Form(
                        attributes: new HtmlAttributes()
                            .Id("PasskeysForm")
                            .Action(Locations.Action(
                                context: context,
                                controller: "Passkeys")),
                        action: () => hb
                                .PasskeysEditor(context: context))
                    .PasskeyChangeTitleDialog(
                        context: context,
                        passkeyId: 0,
                        title: string.Empty)
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            text: Displays.Close(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string Passkeys(Context context)
        {
            return new ResponseCollection()
                .Html(target: "#EditPasskeyWrap",
                    value: new HtmlBuilder()
                        .EditPasskey(context: context))
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder PasskeysEditor(this HtmlBuilder hb, Context context)
        {
            return hb
                .Div(
                    css: "tabs-panel-inner",
                    action: () => hb
                        .Div(css: "command-left", action: () => hb
                            .Button(
                                controlId: "NewPasskey",
                                text: Displays.New(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.send($(this));",
                                icon: "ui-icon-gear",
                                action: "MakeCredentialOptions",
                                method: "post")
                            .Button(
                                controlId: "DeletePasskeys",
                                text: Displays.Delete(context: context),
                                confirm: Displays.ConfirmDelete(context: context),
                                controlCss: "button-icon",
                                onClick: "$p.setAndSend('#EditPasskey', $(this));",
                                icon: "ui-icon-trash",
                                action: "DeleteBulk",
                                method: "post"))
                        .EditPasskey(context: context));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditPasskey(
            this HtmlBuilder hb, Context context)
        {
            var selected = context.Forms.Data("EditPasskey").Deserialize<IEnumerable<int>>();
            return hb
                .GridTable(
                    context: context,
                    id: "EditPasskey",
                    scrollable: false,
                    attributes: new HtmlAttributes()
                        .DataFunc("openPasskeyChangeTitleDialog")
                        .DataName("PasskeyId")
                        .DataMethod("post"),
                    action: () => hb
                        .PasskeysHeader(
                            context: context)
                        .TBody(id: "PasskeyList", action: () =>
                        {
                            PasskeysBody(hb: hb, context: context);
                        }));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder PasskeysHeader(
            this HtmlBuilder hb, Context context)
        {
            var labelAttr = new HtmlAttributes();
            labelAttr.Class("check-option");
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb.CheckBox(controlCss: "select-all"))
                    .Th(action: () => hb.Text(text: Displays.Title(context: context)))
                    .Th(action: () => hb.Text(text: Displays.CreatedTime(context: context)), attributes: new HtmlAttributes().Style("width: 250px"))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder PasskeysBody(
            this HtmlBuilder hb, Context context)
        {
            var list = new PasskeyCollection(
                context: context,
                column: Rds.PasskeysColumn()
                        .PasskeyId()
                        .Title()
                        .CreatedTime(),
                where: Rds.PasskeysWhere()
                        .UserId(context.UserId));
            list.ForEach(passkey =>
            {
                hb.Tr(
                    css: "grid-row",
                    attributes: new HtmlAttributes()
                        .DataId(passkey.PasskeyId.ToString()),
                    action: () => hb
                        .Td(action: () => hb
                            .CheckBox(controlCss: "select"))
                        .Td(action: () => hb
                            .Text(text: passkey.Title))
                        .Td(action: () => hb
                            .Text(text: passkey.CreatedTime.Value.Display(context, "Ymdahm"))));
            });
            return hb;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder PasskeyChangeTitleDialog(
            this HtmlBuilder hb, Context context, int passkeyId, string title)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("PasskeyChangeTitleDialog")
                    .Class("dialog")
                    .Title(Displays.PasskeyEdit(context: context)),
                action: () => hb
                    .PasskeyChangeTitleForm(
                        context: context,
                        passkeyId: passkeyId,
                        title: title));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder PasskeyChangeTitleForm(
            this HtmlBuilder hb, Context context, int passkeyId, string title)
        {
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("PasskeyChangeTitleForm")
                    .Action(Locations.Action(
                        context: context,
                        controller: "Passkeys",
                        id: passkeyId)),
                action: () => hb
                    .FieldTextBox(
                        controlId: nameof(Displays.Passkeys_Title),
                        context: context,
                        labelText: Displays.Title(context: context),
                        text: title)
                    .P(css: "message-dialog")
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "ChangeTitle",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            onClick: "$p.send($(this));",
                            icon: "ui-icon-disk",
                            action: "ChangeTitle",
                            method: "post",
                            dataId: passkeyId.ToString())
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            onClick: "$p.closeDialog($(this));",
                            icon: "ui-icon-cancel")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string ToBase64UrlString(this byte[] bytes)
        {
            return Microsoft.IdentityModel.Tokens.Base64UrlEncoder.Encode(bytes);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static byte[] FromBase64UrlString(this string str)
        {
            return Microsoft.IdentityModel.Tokens.Base64UrlEncoder.DecodeBytes(str);
        }
    }
}
