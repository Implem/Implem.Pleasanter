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
    public static class ScimTokenUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static bool Enabled(Context context)
        {
            return Libraries.Scim.ScimFeatureUtilities.Enabled()
                && (context.HasPrivilege || context.User.TenantManager);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder FieldSetScimToken(this HtmlBuilder hb, Context context)
        {
            return hb.TabsPanelField(
                id: "FieldSetScimToken",
                action: () => hb.ScimTokenSettingsEditor(context: context),
                _using: Enabled(context: context));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ScimTokenSettingsEditor(this HtmlBuilder hb, Context context)
        {
            return hb.FieldSet(id: "ScimTokenSettingsEditor", action: () => hb
                .Div(css: "command-left", action: () => hb
                    .Button(
                        controlId: "NewScimToken",
                        text: Displays.New(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.openScimTokenDialog($(this));",
                        icon: "ui-icon-key",
                        action: "SetScimToken",
                        method: "put")
                    .Button(
                        controlId: "DisableScimTokens",
                        text: Displays.ToDisable(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditScimToken', $(this));",
                        icon: "ui-icon-cancel",
                        action: "SetScimToken",
                        method: "delete",
                        confirm: Displays.ScimConfirmDisableTokens(context: context))
                    .Button(
                        controlId: "DeleteScimTokens",
                        text: Displays.Delete(context: context),
                        controlCss: "button-icon",
                        onClick: "$p.setAndSend('#EditScimToken', $(this));",
                        icon: "ui-icon-trash",
                        action: "SetScimToken",
                        method: "delete",
                        confirm: Displays.ConfirmDelete(context: context)))
                .EditScimToken(context: context));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder EditScimToken(this HtmlBuilder hb, Context context)
        {
            var selected = context.Forms.IntList("EditScimToken");
            return hb.GridTable(
                context: context,
                id: "EditScimToken",
                attributes: new HtmlAttributes()
                    .DataName("ScimTokenId")
                    .DataFunc("openScimTokenDialog")
                    .DataAction("SetScimToken")
                    .DataMethod("post"),
                action: () => hb
                    .EditScimTokenHeader(context: context, selected: selected)
                    .EditScimTokenBody(context: context, selected: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditScimTokenHeader(
            this HtmlBuilder hb, Context context, IEnumerable<int> selected)
        {
            var tokens = ScimTokens(context: context);
            return hb.THead(action: () => hb
                .Tr(css: "ui-widget-header", action: () => hb
                    .Th(action: () => hb
                        .CheckBox(
                            controlCss: "select-all",
                            _checked: tokens.Any()
                                && tokens.All(o => selected?.Contains(o.ScimTokenId) == true)))
                    .Th(action: () => hb.Text(text: Displays.Id(context: context)))
                    .Th(action: () => hb.Text(text: Displays.ScimTokens_TokenPrefix(context: context)))
                    .Th(action: () => hb.Text(text: Displays.Creator(context: context)))
                    .Th(action: () => hb.Text(text: Displays.Disabled(context: context)))
                    .Th(action: () => hb.Text(text: Displays.ScimTokens_ExpiresTime(context: context)))
                    .Th(action: () => hb.Text(text: Displays.ScimTokens_LastUsedTime(context: context)))
                    .Th(action: () => hb.Text(text: Displays.ScimTokens_CreatedTime(context: context)))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder EditScimTokenBody(
            this HtmlBuilder hb, Context context, IEnumerable<int> selected)
        {
            return hb.TBody(action: () =>
                ScimTokens(context: context).ForEach(token => hb
                    .Tr(
                        css: "grid-row",
                        attributes: new HtmlAttributes()
                            .DataId(token.ScimTokenId.ToString()),
                        action: () => hb
                            .Td(action: () => hb
                                .CheckBox(
                                    controlCss: "select",
                                    _checked: selected?.Contains(token.ScimTokenId) == true))
                            .Td(action: () => hb.Text(text: token.ScimTokenId.ToString()))
                            .Td(action: () => hb.Text(text: token.TokenPrefix))
                            .Td(action: () => hb.Text(text: TokenUserName(context: context, userId: token.UserId)))
                            .Td(action: () => hb
                                .Span(
                                    css: "ui-icon ui-icon-circle-check",
                                    _using: token.Disabled == true))
                            .Td(action: () => hb.Text(text: DateText(context, token.ExpiresTime)))
                            .Td(action: () => hb.Text(text: DateText(context, token.LastUsedTime)))
                            .Td(action: () => hb.Text(text: DateText(context, token.CreatedTime.Value))))));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static HtmlBuilder ScimTokenDialog(this HtmlBuilder hb, Context context)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("ScimTokenDialog")
                    .Class("dialog")
                    .Title(Displays.ScimTokens(context: context)),
                _using: Enabled(context: context));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static HtmlBuilder ScimTokenDialog(
            Context context,
            ScimTokenModel scimTokenModel,
            string controlId,
            string issuedToken = null)
        {
            var hb = new HtmlBuilder();
            return hb.Form(
                attributes: new HtmlAttributes()
                    .Id("ScimTokenForm")
                    .Action(Locations.Action(
                        context: context,
                        controller: context.Controller,
                        id: context.TenantId)),
                action: () => hb
                    .FieldText(
                        controlId: "ScimTokenId",
                        controlCss: " always-send",
                        labelText: Displays.Id(context: context),
                        text: scimTokenModel.ScimTokenId.ToString())
                    .FieldText(
                        labelText: Displays.ScimTokens_TokenPrefix(context: context),
                        text: scimTokenModel.TokenPrefix,
                        _using: !scimTokenModel.TokenPrefix.IsNullOrEmpty())
                    .FieldTextBox(
                        context: context,
                        textType: HtmlTypes.TextTypes.DateTime,
                        controlId: "ScimTokenExpiresTime",
                        controlCss: " always-send",
                        labelText: Displays.ScimTokens_ExpiresTime(context: context),
                        format: Displays.YmdhmDatePickerFormat(context: context),
                        text: DateText(context, scimTokenModel.ExpiresTime),
                        validateDate: true)
                    .FieldCheckBox(
                        controlId: "ScimTokenDisabled",
                        fieldCss: "field-normal",
                        controlCss: " always-send",
                        labelText: Displays.Disabled(context: context),
                        _checked: scimTokenModel.Disabled == true)
                    .FieldTextBox(
                        context: context,
                        controlId: "ScimTokenValue",
                        fieldCss: "field-wide",
                        labelText: Displays.Token(context: context),
                        text: issuedToken,
                        attributes: new Dictionary<string, string>
                        {
                            ["readonly"] = "readonly"
                        },
                        _using: !issuedToken.IsNullOrEmpty())
                    .P(
                        css: "message-dialog",
                        action: () => hb.Text(text: !issuedToken.IsNullOrEmpty()
                            ? Displays.ScimTokenOneTimeMessage(context: context)
                            : string.Empty))
                    .Div(css: "command-center", action: () => hb
                        .Button(
                            controlId: "AddScimToken",
                            text: Displays.Add(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-disk",
                            onClick: "$p.setScimToken($(this));",
                            action: "SetScimToken",
                            method: "post",
                            _using: controlId == "NewScimToken")
                        .Button(
                            controlId: "UpdateScimToken",
                            text: Displays.Change(context: context),
                            controlCss: "button-icon validate button-positive",
                            icon: "ui-icon-disk",
                            onClick: "$p.setScimToken($(this));",
                            action: "SetScimToken",
                            method: "post",
                            _using: controlId == "EditScimToken")
                        .Button(
                            text: Displays.Cancel(context: context),
                            controlCss: "button-icon button-neutral",
                            icon: "ui-icon-cancel",
                            onClick: "$p.closeDialog($(this));")));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static string SetScimToken(Context context, SiteSettings ss)
        {
            var res = new ResponseCollection(context: context);
            if (!Enabled(context: context))
            {
                return res
                    .Message(Error.Types.HasNotPermission.Message(context: context))
                    .ToJson();
            }
            switch (context.Forms.ControlId())
            {
                case "NewScimToken":
                case "EditScimToken":
                    OpenScimTokenDialog(context: context, res: res);
                    break;
                case "AddScimToken":
                    AddScimToken(context: context, ss: ss, res: res);
                    break;
                case "UpdateScimToken":
                    UpdateScimToken(context: context, ss: ss, res: res);
                    break;
                case "DisableScimTokens":
                    DisableScimTokens(context: context, res: res);
                    break;
                case "DeleteScimTokens":
                    DeleteScimTokens(context: context, res: res);
                    break;
            }
            return res.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void OpenScimTokenDialog(Context context, ResponseCollection res)
        {
            if (context.Forms.ControlId() == "NewScimToken")
            {
                res.Html("#ScimTokenDialog", ScimTokenDialog(
                    context: context,
                    scimTokenModel: new ScimTokenModel(context: context)
                    {
                        UserId = context.UserId
                    },
                    controlId: context.Forms.ControlId()));
                return;
            }
            var scimTokenModel = GetToken(context: context, scimTokenId: context.Forms.Int("ScimTokenId"));
            if (scimTokenModel == null)
            {
                res.Message(Messages.SelectOne(context: context));
                return;
            }
            res.Html("#ScimTokenDialog", ScimTokenDialog(
                context: context,
                scimTokenModel: scimTokenModel,
                controlId: context.Forms.ControlId()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void AddScimToken(Context context, SiteSettings ss, ResponseCollection res)
        {
            var token = GenerateToken();
            var scimTokenModel = new ScimTokenModel(context: context)
            {
                UserId = context.UserId,
                TokenHash = Convert.ToHexStringLower(
                    inArray: System.Security.Cryptography.SHA256.HashData(
                        source: System.Text.Encoding.UTF8.GetBytes(token))),
                TokenPrefix = token.Length <= 24
                    ? token
                    : token[..24],
                Disabled = context.Forms.Bool("ScimTokenDisabled"),
                ExpiresTime = FormExpiresTime(context: context)
            };
            scimTokenModel.Create(context: context, ss: ss);
            res
                .ReplaceAll("#EditScimTokenWrap", new HtmlBuilder()
                    .EditScimToken(context: context))
                .Html("#ScimTokenDialog", ScimTokenDialog(
                    context: context,
                    scimTokenModel: scimTokenModel,
                    controlId: "EditScimToken",
                    issuedToken: token))
                .Message(Messages.ScimTokenIssued(context: context));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void UpdateScimToken(Context context, SiteSettings ss, ResponseCollection res)
        {
            var scimTokenModel = GetToken(context: context, scimTokenId: context.Forms.Int("ScimTokenId"));
            if (scimTokenModel == null)
            {
                res.Message(Messages.SelectOne(context: context));
                return;
            }
            scimTokenModel.ExpiresTime = FormExpiresTime(context: context);
            scimTokenModel.Disabled = context.Forms.Bool("ScimTokenDisabled");
            scimTokenModel.Update(
                context: context,
                ss: ss,
                param: Rds.ScimTokensParam()
                    .Disabled(scimTokenModel.Disabled)
                    .ExpiresTime(scimTokenModel.ExpiresTime, _using: scimTokenModel.ExpiresTime.InRange())
                    .ExpiresTime(raw: "null", _using: !scimTokenModel.ExpiresTime.InRange()));
            res
                .ReplaceAll("#EditScimTokenWrap", new HtmlBuilder()
                    .EditScimToken(context: context))
                .CloseDialog(target: "#ScimTokenDialog");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static DateTime FormExpiresTime(Context context)
        {
            var value = context.Forms.Data("ScimTokenExpiresTime");
            return value.IsNullOrEmpty()
                ? 0.ToDateTime()
                : value.ToDateTime().ToUniversal(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void DisableScimTokens(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditScimToken");
            if (selected?.Count <= 0)
            {
                res.Message(Messages.SelectTargets(context: context));
                return;
            }
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.UpdateScimTokens(
                    param: Rds.ScimTokensParam()
                        .Disabled(true),
                    where: Rds.ScimTokensWhere()
                        .TenantId(context.TenantId)
                        .ScimTokenId_In(selected)));
            res.ReplaceAll("#EditScimTokenWrap", new HtmlBuilder()
                .EditScimToken(context: context));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static void DeleteScimTokens(Context context, ResponseCollection res)
        {
            var selected = context.Forms.IntList("EditScimToken");
            if (selected?.Count <= 0)
            {
                res.Message(Messages.SelectTargets(context: context));
                return;
            }
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteScimTokens(
                    where: Rds.ScimTokensWhere()
                        .TenantId(context.TenantId)
                        .ScimTokenId_In(selected)));
            res.ReplaceAll("#EditScimTokenWrap", new HtmlBuilder()
                .EditScimToken(context: context));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static ScimTokenModel GetToken(Context context, int scimTokenId)
        {
            if (scimTokenId <= 0)
            {
                return null;
            }
            var scimTokenModel = new ScimTokenModel(
                context: context,
                scimTokenId: scimTokenId);
            return scimTokenModel.AccessStatus == Databases.AccessStatuses.Selected
                && scimTokenModel.TenantId == context.TenantId
                    ? scimTokenModel
                    : null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static ScimTokenCollection ScimTokens(Context context)
        {
            return new ScimTokenCollection(
                context: context,
                where: Rds.ScimTokensWhere()
                    .TenantId(context.TenantId),
                orderBy: Rds.ScimTokensOrderBy()
                    .ScimTokenId(SqlOrderBy.Types.desc));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string GenerateToken()
        {
            var secret = Convert.ToHexStringLower(
                inArray: System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
            return $"pleasanter.scim.{secret}";
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string DateText(Context context, DateTime value)
        {
            return value.InRange()
                ? value.ToLocal(context: context, Displays.YmdhmsFormat(context: context))
                : string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string TokenUserName(Context context, int userId)
        {
            var name = SiteInfo.User(context: context, userId: userId)?.Name;
            return name.IsNullOrEmpty()
                ? userId.ToString()
                : name;
        }
    }
}
